using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine
{
    public struct MemorisedValue
    {
        public readonly string StepName;
        public object? Value;

        public MemorisedValue(string stepName, object? value)
        {
            StepName = stepName;
            Value = value;
        }
    }
}
