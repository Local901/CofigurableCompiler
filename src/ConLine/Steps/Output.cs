using ConLine.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.Steps
{
    public class Output<TType> : Output
    {
        public Output(string name, bool isOptional = false)
            : base(name, typeof(TType), isOptional)
        { }
    }
    public class Output : IStep
    {
        public string Name { get; }

        public IReadOnlyList<IIOType> Outputs { get; }

        public IReadOnlyList<IIOType> Inputs { get; }

        public Output(string name, Type type, bool isOptional = false)
            : this(
                  name,
                  new IOType(name, type, isOptional, null),
                  new IOType(name, type, isOptional, null)
              )
        { }
        public Output(string name, IIOType input, IIOType output)
        {
            Name = name;
            Inputs = new List<IIOType>() { input };
            Outputs = new List<IIOType>() { output };
        }

        public Task<StepValue[]> Run(RunOptions options, InputOptions input)
        {
            return new Task<StepValue[]>(() =>
            {
                return new StepValue[]
                {
                    new StepValue<object>(Name, input.StepValues[0])
                };
            });
        }
    }
}
