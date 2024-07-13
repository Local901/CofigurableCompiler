using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine
{
    public struct Connection
    {
        public readonly string StepName;
        public readonly string PropertyName;

        public Connection(string stepName, string propertyName)
        {
            StepName = stepName;
            PropertyName = propertyName;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null) return false;
            if (obj is Connection c2)
            {
                return StepName == c2.StepName && PropertyName == c2.PropertyName;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StepName, PropertyName);
        }

        public override string ToString()
        {
            return $"{StepName}::{PropertyName}";
        }
    }
}
