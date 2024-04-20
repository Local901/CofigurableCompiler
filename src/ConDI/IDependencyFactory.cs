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
        /// <returns>That instance if it could be created.</returns>
        public TInstance? GetInstance<TInstance>();

        /// <summary>
        /// Create a new object of the requested type.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance to be created.</typeparam>
        /// <returns>The object of that instance type if it could be created.</returns>
        public TInstance? CreateInstance<TInstance>();

        /// <summary>
        /// Returns true when the dependency is known in the injector.
        /// </summary>
        /// <typeparam name="TDepenedency">The type of the dependency.</typeparam>
        /// <returns></returns>
        public bool IsKnownDependency<TDepenedency>();

        /// <summary>
        /// Get the dependency object that is storing the instance of the correct scope.
        /// Priority of dependency types is Tansiant before Scoped before Singleton.
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <exception cref="Exception">The dependency is unknown in the scopes.</exception>
        /// <returns></returns>
        public Dependency<TDependency> GetDependency<TDependency>();
    }
}
