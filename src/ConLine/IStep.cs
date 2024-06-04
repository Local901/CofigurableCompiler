using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine
{
    public interface IStep
    {
        public string Name { get; }
        public IReadOnlyList<IIOType> Outputs { get; }
        public IReadOnlyList<IIOType> Inputs { get; }

        public Task Run(RunOptions options, IStepInput input);
    }
}
