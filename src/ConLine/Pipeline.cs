using ConDI;
using ConLine.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine
{
    public abstract class Pipeline : IPipeLine
    {
        public string Name { get; }
        protected Injector Injector = new();
        public IInjectorSetup InjectorSetup => Injector;
        public bool CanBeEdited { get; protected set; } = true;
        private IReadOnlyList<IIOType>? _outputs;
        public IReadOnlyList<IIOType> Outputs
        {
            get
            {
                if (_outputs != null && !CanBeEdited) return _outputs;
                CanBeEdited = false;
                _outputs = GetOutputSteps()
                    .SelectMany((step) => step.Outputs.ToArray())
                    .ToList();
                return _outputs;
            }
        }
        private IReadOnlyList<IIOType>? _inputs;
        public IReadOnlyList<IIOType> Inputs
        {
            get
            {
                if (_inputs != null) return _inputs;
                CanBeEdited = false;
                _inputs = GetInputSteps()
                    .SelectMany((step) => step.Inputs.ToArray())
                    .ToList();
                return _inputs;
            }
        }

        protected Dictionary<string, IStep> Steps { get; } = new Dictionary<string, IStep>();
        protected Dictionary<Connection, IList<Connection>> Connections { get; } = new Dictionary<Connection, IList<Connection>>();

        /// <summary>
        /// Create an abstract pipeline.
        /// </summary>
        /// <param name="name"></param>
        public Pipeline(string name)
        {
            Name = name;
        }

        public void AddConnection(Connection from, Connection to)
        {
            if (!CanBeEdited)
            {
                throw new Exception($"Pipeline {Name}: New connections are no longer allowed. Connection {from} -> {to}");
            }
            var connections = Connections[from];
            if (connections == null)
            {
                connections = new List<Connection>();
                Connections[from] = connections;
            }
            connections.Add(to);
        }

        public void AddStep(IStep step)
        {
            if (!CanBeEdited)
            {
                throw new Exception($"Pipeline {Name}: New steps are no longer allowed. Step {step.Name}");
            }
            if (Steps.ContainsKey(step.Name))
            {
                throw new Exception($"Pipeline {Name}: Step already defined. Step {step.Name}");
            }
            Steps[step.Name] = step;
        }

        public IStep? GetStep(string name)
        {
            return Steps[name];
        }

        public Input[] GetInputSteps()
        {
            return Steps.Values.OfType<Input>().ToArray();
        }

        public Output[] GetOutputSteps()
        {
            return Steps.Values.OfType<Output>().ToArray();
        }

        public abstract Task Run(RunOptions options, IStepInput input);
    }
}
