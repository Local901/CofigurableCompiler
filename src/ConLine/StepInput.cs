using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine
{
    public class StepInput : IStepInput
    {
        public IReadOnlyList<IInputValue> Values { get; }

        private Action<string, object?> Callback { get; }

        public StepInput(IReadOnlyList<IInputValue> values, Action<string, object?> callback)
        {
            Values = values;
            Callback = callback;
        }

        public void SendResult(string resultName, object? value)
        {
            Callback(resultName, value);
        }
    }
}
