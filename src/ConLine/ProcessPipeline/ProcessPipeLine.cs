using ConDI;
using ConLine.Steps;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.ProcessPipeline
{
    public class ProcessPipeLine : Pipeline
    {
        public ProcessPipeLine(string name)
            : base(name) { }

        private class WaitingSteps
        {
            public IStep Step;
            public List<IInputValue> Values;

            public WaitingSteps(IStep step)
            {
                Step = step;
                Values = new List<IInputValue>();
            }
        }

        public override async Task Run(RunOptions options, IStepInput input)
        {
            CanBeEdited = false;
            var stack = new LinkedList<Task>();
            var memory = new LinkedList<WaitingSteps>();

            // initialize starting steps.
            {
                foreach (var step in Steps.Values.OfType<Input>())
                {
                    var task = step.Run(
                        options,
                        new StepInput(
                            input.Values,
                            CreateCallback(step, memory, input.SendResult)
                        )
                    );
                }
            }

            // Processing loop
            while (stack.Count > 0 || memory.Count > 0)
            {
                // Start new steps.
                var node = memory.First;
                while (node != null)
                {
                    var waitingStep = node.Value;
                    var readyInputNames = waitingStep.Values.Select((v) => v.Name).ToArray();
                    if (!waitingStep.Step.Inputs.All((input) => readyInputNames.Contains(input.Name)))
                    {
                        node = node.Next;
                        continue;
                    }

                    var task = waitingStep.Step.Run(
                        options,
                        new StepInput(
                            waitingStep.Values,
                            CreateCallback(waitingStep.Step, memory, input.SendResult)
                        )
                    );

                    // Syncronyse process.
                    if (options.RunSyncronous)
                    {
                        await task;
                        continue;
                    }

                    stack.AddLast(task);

                    // Remove started steps from memory.
                    var next = node.Next;
                    memory.Remove(node);
                    node = next;
                }

                // Remove completed steps.
                foreach (var item in stack)
                {
                    if (item.IsFaulted && item.Exception != null)
                    {
                        throw item.Exception;
                    }

                    if (item.IsCompleted)
                    {
                        stack.Remove(item);
                        return;
                    }
                }
            }
        }

        private Action<string, object?> CreateCallback(IStep step, LinkedList<WaitingSteps> memories, Action<string, object?> callback)
        {
            if (step is Output)
            {
                return callback;
            }
            return (outputName, value) =>
            {
                // Find all connections connected to this step::outputname.
                var connectedInputs = Connections[new Connection(step.Name, outputName)];

                if (connectedInputs == null || connectedInputs.Count == 0)
                {
                    // Drop unused values.
                    return;
                }

                foreach (var connectedInput in connectedInputs)
                {
                    // Find first waiting memeory for the connected step without a value for the connected property.
                    var memory = memories.FirstOrDefault((mem) => 
                        mem.Step.Name == connectedInput.StepName &&
                        mem.Values.All((item) => item.Name != connectedInput.PropertyName)
                    );

                    if (memory == null)
                    {
                        memory = new WaitingSteps(Steps[connectedInput.StepName]);
                        memories.AddLast(memory);
                    }

                    // Could output more than one value wich should cause it change the id.
                    memory.Values.Add(new InputValue(outputName, value));
                }
            };
        }
    }
}
