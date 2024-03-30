using System;
using System.Collections.Generic;
using System.Text;

namespace ConDI
{
    public struct Dependency
    {
        public Type Type { get; }
        public DependencyType DependencyType { get; }

        public Dependency(Type type, DependencyType dependencyType)
        {
            Type = type;
            DependencyType = dependencyType;
        }
    }
}
