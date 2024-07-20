using ConLine.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.Steps
{
    public class Input<TType> : Input
    {
        public Input(string name, bool isOptional = false)
            : base(name, typeof(TType), isOptional) { }
    }
    public class Input : IStep
    {
        public string Name { get; }

        public IReadOnlyList<IIOType> Outputs { get; }

        public IReadOnlyList<IIOType> Inputs { get; }
        public Input(string name, Type type, bool isOptional = false)
            : this(
                  name,
                  new IOType(name, type, isOptional, null),
                  new IOType(name, type, isOptional, null)
              )
        { }
        public Input(string name, IIOType input, IIOType? output)
        {
            Name = name;
            Inputs = new List<IIOType>() { input };
            Outputs = new List<IIOType>() { output ?? input };
        }

        public Task<StepValue[]> Run(RunOptions options, InputOptions input)
        {
            return new Task<StepValue[]>(() =>
            {
                var value = input.StepValues.First((i) => i.PropertyName == Inputs[0].Name);
                return new StepValue[] {
                    new StepValue<object>(Outputs[0].Name, value.GetAs<object>().Value)
                };
            });
        }
    }
}
