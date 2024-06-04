using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine
{
    public class IOType : IIOType
    {
        public string Name { get; }
        public Type Type { get; }
        public bool IsOptional { get; }
        public object? DefaultValue { get; }

        public IOType(string name, Type type, bool isOptional, object? defaultValue)
        {
            Name = name;
            Type = type;
            IsOptional = isOptional;
            DefaultValue = defaultValue;
        }
    }

    public class IOType<TType> : IOType
    {
        public new TType? DefaultValue { get => (TType?)base.DefaultValue; }

        public IOType(string name, bool isOptional, TType? defaultValue)
            :base(name, typeof(TType), isOptional, defaultValue)
        {

        }
    }
}
