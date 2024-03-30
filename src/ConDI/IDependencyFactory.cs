using System;
using System.Collections.Generic;
using System.Text;

namespace ConDI
{
    public interface IDependencyFactory
    {
        /// <summary>
        /// Get the instance of an object.
        /// </summary>
        /// <typeparam name="TInstance">The wanted instance type.</typeparam>
        /// <returns>That instance.</returns>
        public TInstance GetInstance<TInstance>();

        /// <summary>
        /// Create a new instance of this type.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <exception cref="Exception">A exception</exception>
        /// <returns>The created instance.</returns>
        public TInstance CreateInstance<TInstance>();
    }
}
