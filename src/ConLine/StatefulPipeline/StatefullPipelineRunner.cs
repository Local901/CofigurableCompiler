using ConLine.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.StatefulPipeline
{
    public class StatefullPipelineRunner
    {
        private List<RunningStepMemory> RunningSteps = new List<RunningStepMemory>();
        private List<WaitingStepMemory> WaitingSteps = new List<WaitingStepMemory>();
        private List<MemorisedValue> RuntimeMemory = new List<MemorisedValue>();

        private RunOptions RunOptions;
        private IStepInput StepInput;
        private IPipeLine Pipeline;

        private StepValue[] Output;

        public StatefullPipelineRunner(
            RunOptions options,
            IStepInput input,
            IPipeLine pipeLine
        )
        {
            RunOptions = options;
            StepInput = input;
            Pipeline = pipeLine;
            Output = new StepValue[pipeLine.GetOutputSteps().Length];
        }

        public async Task<StepValue[]> Run()
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

            return Output;
        }

        private void HandleFinishedStep(IStep step, StepValue[] result)
        {
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
                    waitingStep.SetInputValue(value);

                    // TODO: check for inputs from memory steps. (They must be defined before this step)

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
                                    waitingStep.InputValues.ToArray()
                                )
                            )
                        );
                    }
                }
            }
        }
    }
}
