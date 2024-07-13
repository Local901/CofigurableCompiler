using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.StatefulPipeline
{
    public struct RunningStepMemory
    {
        public readonly IStep Step;
        public readonly Task<StepValue[]> RunningTask;

        public RunningStepMemory(IStep step, Task<StepValue[]> task)
        {
            Step = step;
            RunningTask = task;
        }
    }
}
