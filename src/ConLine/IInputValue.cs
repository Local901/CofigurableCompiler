using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine
{
    public interface IInputValue
    {
        public string Name { get; }
        public object? Value { get; }
    }
}
