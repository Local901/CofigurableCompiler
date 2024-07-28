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

        /// <summary>
        /// Create an input step. This is a special step that filters the input of a pipeline.
        /// The name of the input and output proerty is the same as the name of the step.
        /// </summary>
        /// <param name="name">Name of the step.</param>
        /// <param name="type">Type of the input.</param>
        /// <param name="isOptional">Is the input optional.</param>
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
            var task = new Task<StepValue[]>(() =>
            {
                var value = input.StepValues.First((i) => i.PropertyName == Inputs[0].Name);
                return new StepValue[] {
                    new StepValue<object>(Outputs[0].Name, value.GetValueAs<object>())
                };
            });
            task.Start();
            return task;
        }
    }
}
