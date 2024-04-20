using System;
using System.Collections.Generic;
using System.Text;

namespace ConDI
{
    public interface IDependency
    {
        public DependencyProperties Properties { get; }
        public object? Instance { get; }
        public object? CreateInstance();
    }
    public interface IDependency<T> : IDependency
    {
        public new T? Instance { get; }
        public new T? CreateInstance();
    }
}
