using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine
{
    public abstract class StepValue
    {
        public readonly string PropertyName;

        public StepValue(string propertyName)
        {
            PropertyName = propertyName;
        }

        public abstract bool IsArray { get; }
        public abstract Type Type { get; }

        /// <summary>
        /// Return a step output object of the provided type if the value is of that type.
        /// </summary>
        /// <typeparam name="TType">The expected type.</typeparam>
        /// <returns>StepOutput of the provided type if the value is of that type.</returns>
        public abstract StepValue<TType>? GetAs<TType>();
    }

    public class StepValue<TType>: StepValue
    {
        public readonly TType Value;
        public override Type Type => Value?.GetType() ?? typeof(TType);
        public override bool IsArray => Type.IsArray;

        public StepValue(string propertyName, TType value)
            : base(propertyName)
        {
            Value = value;
        }

        public override StepValue<TType1>? GetAs<TType1>()
        {
            if (Value is TType1 newValue)
            {
                return new StepValue<TType1>(PropertyName, newValue);
            }
            return null;
        }
    }
}
