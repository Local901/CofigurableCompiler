using System;
using System.Collections.Generic;
using System.Text;

namespace ConDI
{
    public struct DependencyProperties
    {
        public Type Type { get; }
        public DependencyType DependencyType { get; }

        public DependencyProperties(Type type, DependencyType dependencyType)
        {
            Type = type;
            DependencyType = dependencyType;
        }
    }
}
