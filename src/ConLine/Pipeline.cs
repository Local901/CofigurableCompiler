using ConDI;
using ConLine.Options;
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
        public Injector Injector { get; } = new();
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
        protected Dictionary<Connection, List<Connection>> ConnectionsFrom { get; } = new Dictionary<Connection, List<Connection>>();
        protected Dictionary<Connection, Connection> ConnectionTo { get; } = new Dictionary<Connection, Connection>();

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
            if (ConnectionTo.ContainsKey(to))
            {
                throw new Exception($"There is already a connection to {to} from {ConnectionTo[to]}");
            }
            ConnectionTo[to] = from;

            List<Connection>? connections;
            if (!ConnectionsFrom.TryGetValue(from, out connections))
            {
                connections = new List<Connection>();
                ConnectionsFrom[from] = connections;
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

        public IReadOnlyList<Connection> GetConnectionsFrom(Connection from)
        {
            List<Connection> connections;
            if (ConnectionsFrom.TryGetValue(from, out connections))
            {
                return connections;
            }
            return Array.Empty<Connection>();
        }

        public Connection? GetConnectionTo(Connection to)
        {
            Connection connection;
            if (ConnectionTo.TryGetValue(to, out connection))
            {
                return connection;
            }
            return null;
        }

        public abstract Task<StepValue[]> Run(RunOptions options, InputOptions input);
    }
}
