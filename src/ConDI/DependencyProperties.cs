using ConDI.InstanceFactories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConDI
{
    public struct DependencyProperties
    {
        public Type Type => InstanceFactory.InstanceType;
        public InstanceScope InstanceScope { get; }
        public IInstanceFactory InstanceFactory { get; }

        public DependencyProperties(InstanceScope instanceScope, IInstanceFactory instanceFactory)
        {
            InstanceScope = instanceScope;
            InstanceFactory = instanceFactory;
        }
    }
}
