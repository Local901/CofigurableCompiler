using ConDI;
using ConLine.Steps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

        /// <summary>
        /// Get the value as the wanted type.
        /// </summary>
        /// <typeparam name="TType">Type to return.</typeparam>
        /// <exception cref="Exception">When the wanted type is not related to the type of the value.</exception>
        /// <returns>The object of that type.</returns>
        public abstract TType GetValueAs<TType>();

        /// <summary>
        /// Create an instance of <see cref="StepValue"/>.
        /// </summary>
        /// <param name="type">The type of the value.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns>A new Step Value.</returns>
        public static StepValue CreateInstance(IDependencyFactory factory, Type type, string propertyName, object value)
        {
            return (StepValue)factory.CreateInstance(
                typeof(StepValue<>).MakeGenericType(new Type[] { type }),
                new KeyValuePair<string, object>[] {
                    new KeyValuePair<string, object>("propertyName", propertyName),
                    new KeyValuePair<string, object>("value", value)
            });
        }
    }

    public class StepValue<TType> : StepValue
    {
        public TType Value;
        public override Type Type => Value?.GetType() ?? typeof(TType);
        public override bool IsArray => Type.IsArray;

        public StepValue(string propertyName, TType value)
            : base(propertyName)
        {
            Value = value;
        }

        public override StepValue<TType1>? GetAs<TType1>()
        {
            return this as StepValue<TType1>;
        }

        public override TType1 GetValueAs<TType1>()
        {
            if (Value is TType1 v1 && v1 is TType)
            {
                return v1;
            }
            throw new Exception($"{nameof(TType1)} is not an instance of {nameof(TType)}");
        }
    }
}
