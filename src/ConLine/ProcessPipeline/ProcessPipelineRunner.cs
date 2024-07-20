using ConDI;
using ConLine.Options;
using ConLine.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.ProcessPipeline
{
    public class ProcessPipelineRunner
    {
        private List<RunningStepMemory> RunningSteps = new List<RunningStepMemory>();
        private List<WaitingStepMemory> WaitingSteps = new List<WaitingStepMemory>();
        private List<MemorisedValue> RuntimeMemory = new List<MemorisedValue>();

        private RunOptions RunOptions;
        private InputOptions StepInput;
        private IPipeLine Pipeline;
        private Scope PipelineScope;

        private StepValue[] Output;

        public ProcessPipelineRunner(
            RunOptions options,
            InputOptions input,
            IPipeLine pipeLine
        )
        {
            RunOptions = options;
            StepInput = input;
            Pipeline = pipeLine;
            PipelineScope = Pipeline.Injector.CreateScope(StepInput.Scope);
            Output = pipeLine.Outputs.Select((s) => StepValue.CreateInstance(PipelineScope, s.Type, s.Name, null)).ToArray();
        }

        public Task<StepValue[]> Run()
        {
            // Initialize
            foreach (var input in Pipeline.GetInputSteps())
            {
                var runningTask = input.Run(RunOptions, StepInput);
                RunningSteps.Add(new RunningStepMemory(input, runningTask));
            }

            // Processing loop
            while(RunningSteps.Count > 0)
            {
                Task.WaitAny(RunningSteps.Select((mem) => mem.RunningTask).ToArray());

                RunningSteps.RemoveAll((mem) =>
                {
                    var runningStep = mem.RunningTask;
                    if (!runningStep.IsCompleted)
                    {
                        return false;
                    }
                    if (runningStep.IsFaulted)
                    {
                        Console.WriteLine(runningStep.Exception.ToString());
                        return true;
                    }
                    if (!runningStep.IsCanceled) return true;

                    var output = runningStep.Result;
                    HandleFinishedStep(mem.Step, output);

                    return true;
                });
            }

            if (WaitingSteps.Count > 0)
            {
                Console.WriteLine($"Pipeline {Pipeline.Name}: Fininshed running with waiting steps. {WaitingSteps.Count}");
            }

            return Task.FromResult(Output);
        }

        private void HandleFinishedStep(IStep step, StepValue[] result)
        {
            // Set output value
            if (step is Output)
            {
                for (int i = 0; i < Pipeline.Outputs.Count; i++)
                {
                    var value = result.FirstOrDefault((r) => r.PropertyName == Pipeline.Outputs[i].Name);
                    if (value == null) return;
                    Output[i] = value;
                }
            }

            foreach(var value in result)
            {
                var connections = Pipeline.GetConnectionsFrom(
                    new Connection(step.Name, value.PropertyName)
                );
                foreach(var connection in connections)
                {
                    IStep? nextStep = Pipeline.GetStep(connection.StepName);
                    if (nextStep == null)
                    {
                        throw new Exception($"Pipeline {Pipeline.Name}: Connected step doesn't exists {connection.StepName}");
                    }

                    var waitingStep = WaitingSteps.FirstOrDefault((mem) => mem.Step.Name == connection.StepName);
                    if (waitingStep == null)
                    {
                        waitingStep = new WaitingStepMemory(nextStep);
                        WaitingSteps.Add(waitingStep);

                    }
                    // Add input value with correct property name.
                    waitingStep.SetInputValue(StepValue.CreateInstance(PipelineScope, value.Type, connection.PropertyName, value.GetValueAs<object>()));

                    // TODO: check for inputs from memory steps. (They must be defined before this step)
                    // Or set value in waiting steps when memory is set.

                    // check if step can be started.
                    var connectedInputs = nextStep.Inputs.Select<IIOType, Connection?>((i) => Pipeline.GetConnectionTo(new Connection(nextStep.Name, i.Name)) != null
                        ? new Connection(nextStep.Name, i.Name)
                        : null
                    );
                    if (
                        // All connected inputs have a value
                        connectedInputs.All((c) => waitingStep.InputValues.Any((v) => v?.PropertyName == c?.PropertyName)) &&
                        // AND All non optional inputs have a value
                        nextStep.Inputs.All((i) => i.IsOptional || connectedInputs.Contains(new Connection(nextStep.Name, i.Name)))
                    ) {
                        RunningSteps.Add(
                            new RunningStepMemory(
                                nextStep,
                                nextStep.Run(
                                    RunOptions,
                                    CreateInputOptions(nextStep, waitingStep.InputValues.ToArray())
                                )
                            )
                        );
                    }
                }
            }
        }

        private InputOptions CreateInputOptions(IStep step, StepValue[] values)
        {
            StepValue? outputField = step is Output
                ? Output.FirstOrDefault((o) => step.Outputs[0].Name == o.PropertyName)
                : null;
            MemorisedValue? memoryField = null;
            if (step is Memory)
            {
                memoryField = RuntimeMemory.FirstOrDefault((m) => m.StepName == step.Name);
                if (memoryField == null)
                {
                    memoryField = new MemorisedValue(step.Name, null);
                    RuntimeMemory.Add((MemorisedValue)memoryField);
                }
            }
            return new CompleteInputOptions(PipelineScope.CreateScope(), values, outputField, memoryField);
        }
    }
}
