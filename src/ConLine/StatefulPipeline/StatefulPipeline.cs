using ConDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.StatefulPipeline
{
    public class StatefulPipeline : Pipeline
    {
        public StatefulPipeline(string name)
            : base(name) { }

        public override Task<StepValue[]> Run(RunOptions options, IStepInput input)
        {
            CanBeEdited = false;

            var runningSteps = new List<Task>();
            var waitingSteps = new List<WaitingStepMemory>();
            var runtimeMemory = new List<MemorisedValue>();



            throw new NotImplementedException();
        }

        private void InitializeMemory(IList<WaitingStepMemory> memory, IStepInput input)
        {
            
        }
    }
}
