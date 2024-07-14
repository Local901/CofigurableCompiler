using ConLine.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.Steps
{
    public class Memory : IStep
    {
        public string Name { get; }

        public IReadOnlyList<IIOType> Outputs { get; }

        public IReadOnlyList<IIOType> Inputs { get; }

        public Memory(string name, Type type)
        {
            Name = name;
            Inputs = new IIOType[] { new IOType("value", type, false, null) };
            Outputs = new IIOType[] { new IOType("value", type, false, null) };
        }

        public Task<StepValue[]> Run(RunOptions options, InputOptions input)
        {
            throw new NotImplementedException();
        }
    }
}
