using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.Steps
{
    public class Input : IStep
    {
        public string Name { get; }

        public IReadOnlyList<IIOType> Outputs { get; }

        public IReadOnlyList<IIOType> Inputs { get; }

        public Input(string name, IIOType input, IIOType output)
        {
            Name = name;
            Inputs = new List<IIOType>() { input };
            Outputs = new List<IIOType>() { output };
        }

        public Task Run(RunOptions options, IStepInput input)
        {
            return new Task(() =>
            {
                var value = input.Values.First((i) => i.Name == Inputs[0].Name);
                input.SendResult(Outputs[0].Name, value);
            });
        }
    }
}
