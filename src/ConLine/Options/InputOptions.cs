using ConDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.Options
{
    public interface InputOptions
    {
        public StepValue[] StepValues { get; }
        public Scope Scope { get; }
    }
}
