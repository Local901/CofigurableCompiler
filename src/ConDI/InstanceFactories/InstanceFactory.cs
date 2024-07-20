using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConDI.InstanceFactories
{
    public interface IInstanceFactory
    {
        public Type InstanceType { get; }

        /// <summary>
        /// Create a instance of the type.
        /// </summary>
        /// <param name="scope">Scope to help create extra objects.</param>
        /// <returns>The created instance.</returns>
        public object? CreateInstance(IScope scope);
    }

    public abstract class InstanceFactory<TInstance> : IInstanceFactory
    {
        public Type InstanceType => typeof(TInstance);

        /// <summary>
        /// Create a instance of the instance type.
        /// </summary>
        /// <param name="scope">Scope to help create extra objects.</param>
        /// <returns>The created instance.</returns>
        public abstract TInstance? CreateInstance(IScope scope);

        object? IInstanceFactory.CreateInstance(IScope scope)
        {
            return CreateInstance(scope);
        }
    }
}
