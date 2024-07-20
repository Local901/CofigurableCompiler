using ConDI;
using ConLine.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.ProcessPipeline
{
    public class ProcessPipeline : Pipeline
    {
        public ProcessPipeline(string name)
            : base(name) { }

        public override Task<StepValue[]> Run(RunOptions options, InputOptions input)
        {
            CanBeEdited = false;

            // TODO: check state of pipeline.

            var runner = new ProcessPipelineRunner(options, input, this);
            return runner.Run();
        }
    }
}
