using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.Steps
{
    public class Output : IStep
    {
        public string Name { get; }

        public IReadOnlyList<IIOType> Outputs { get; }

        public IReadOnlyList<IIOType> Inputs { get; }

        public Output(string name, IIOType input, IIOType output)
        {
            Name = name;
            Inputs = new List<IIOType>() { input };
            Outputs = new List<IIOType>() { output };
        }

        public Task Run(RunOptions options, IStepInput input)
        {
            return new Task(() =>
            {
                foreach (var v in input.Values)
                {
                    input.SendResult(Outputs[0].Name, v);
                }
            });
        }
    }
}
