using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine
{
    public class InputValue : IInputValue
    {
        public string Name { get; }
        public object? Value { get; }

        public InputValue(string name, object? value)
        {
            Name = name;
            Value = value;
        }
    }
}
