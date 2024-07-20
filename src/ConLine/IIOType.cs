using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine
{
    public interface IIOType
    {
        public string Name { get; }
        public Type Type { get; }
        public bool IsOptional { get; }
        public object? DefaultValue { get; }
    }
}
