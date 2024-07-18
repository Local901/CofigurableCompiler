using System;
using System.Collections.Generic;
using System.Text;

namespace ConDI
{
    public interface IDependency
    {
        public InstanceScope InstanceScope { get; }
        public object? GetInstance();
    }
    public interface IDependency<TInstance> : IDependency
    {
        public new TInstance? GetInstance();
    }
}
