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
    public class ProcessPipeLine : IPipeLine
    {
        public string Name { get; }

        private IReadOnlyList<IIOType>? _output;
        public IReadOnlyList<IIOType> Outputs
        {
            get
            {
                if (_output != null) return _output;
                CanBeEdited = false;
                _output = Steps.Values.OfType<Output>()
                    .SelectMany((step) => step.Outputs.ToArray())
                    .ToList();
                return _output;
            }
        }
        private IReadOnlyList<IIOType>? _input;
        public IReadOnlyList<IIOType> Inputs
        {
            get
            {
                if (_input != null) return _input;
                CanBeEdited = false;
                _input = Steps.Values.OfType<Input>()
                    .SelectMany((step) => step.Inputs.ToArray())
                    .ToList();
                return _input;
            }
        }

        public Dictionary<string, IStep> Steps { get; } = new Dictionary<string, IStep>();
        public Dictionary<Connection, IList<Connection>> Connections { get; } = new Dictionary<Connection, IList<Connection>>();

        public bool CanBeEdited { get; private set; } = true;

        private Injector Injector = new();

        public IInjectorSetup InjectorSetup => Injector;

        public ProcessPipeLine(string name)
        {
            Name = name;
        }

        public void AddConnection(Connection from, Connection to)
        {
            if (!CanBeEdited) throw new Exception($"Pipeline {Name}: The state of the pipe line is already fixed.");
            IList<Connection>? toConnections;
            if (!Connections.TryGetValue(from, out toConnections))
            {
                toConnections = new List<Connection>();
                Connections.Add(from, toConnections);
            }
            toConnections.Add(to);
        }

        public void AddStep(IStep step)
        {
            if (!CanBeEdited) throw new Exception($"Pipeline {Name}: The state of the pipe line is already fixed.");
            if (Steps.ContainsKey(step.Name))
            {
                throw new Exception($"PipeLine \"{Name}\": Already contains step with name \"{step.Name}\"");
            }
            Steps[step.Name] = step;
        }

        public IStep? GetStep(string name)
        {
            return Steps[name];
        }

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

        public async Task Run(RunOptions options, IStepInput input)
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
