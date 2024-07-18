using ConDI.InstanceFactories;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ConDI
{
    public class Dependency<TInstance> : IDependency<TInstance>
    {
        private TInstance? Instance;
        public InstanceScope InstanceScope { get; }
        private readonly InstanceFactory<TInstance> Factory;
        private readonly IScope Scope;

        public Dependency(InstanceScope instanceScope, InstanceFactory<TInstance> factory, IScope scope)
        {
            InstanceScope = instanceScope;
            Factory = factory;
            Scope = scope;
        }

        public TInstance? GetInstance()
        {
            return Instance ??= Factory.CreateInstance(Scope);
        }

        object? IDependency.GetInstance()
        {
            return GetInstance();
        }
    }
}
