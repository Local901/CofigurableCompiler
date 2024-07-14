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
        /// Get the instance of an object.
        /// </summary>
        /// <param name="type">The wanted instance type.</param>
        /// <returns>That instance if it could be created.</returns>
        public object? GetInstance(Type type);

        /// <summary>
        /// Create a new object of the requested type.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance to be created.</typeparam>
        /// <returns>The object of that instance type if it could be created.</returns>
        public TInstance? CreateInstance<TInstance>();
        /// <summary>
        /// Create a new object of the requested type posibly using the provided properties.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance to be created.</typeparam>
        /// <param name="propertyValues">Property name value pairs.</param>
        /// <returns>The object of that instance type if it could be created.</returns>
        public TInstance? CreateInstance<TInstance>(KeyValuePair<string, object>[] propertyValues);
        /// <summary>
        /// Create a new object of the requested type.
        /// </summary>
        /// <param name="type">The type of the instance to be created.</param>
        /// <returns>The object of that instance type if it could be created.</returns>
        public object? CreateInstance(Type type);
        /// <summary>
        /// Create a new object of the requested type posibly using the provided properties.
        /// </summary>
        /// <param name="type">The type of the instance to be created.</param>
        /// <param name="propertyValues">Property name value pairs.</param>
        /// <returns>The object of that instance type if it could be created.</returns>
        public object? CreateInstance(Type type, KeyValuePair<string, object>[] propertyValues);

        /// <summary>
        /// Returns true when the dependency is known in the injector.
        /// </summary>
        /// <typeparam name="TDepenedency">The type of the dependency.</typeparam>
        /// <returns></returns>
        public bool IsKnownDependency<TDepenedency>();
        /// <summary>
        /// Returns true when the dependency is known in the injector.
        /// </summary>
        /// <param name="type">The type of the dependency.</param>
        /// <returns></returns>
        public bool IsKnownDependency(Type type);

        /// <summary>
        /// Get the dependency object that is storing the instance of the correct scope.
        /// Priority of dependency types is Tansiant before Scoped before Singleton.
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <exception cref="Exception">The dependency is unknown in the scopes.</exception>
        /// <returns></returns>
        public Dependency<TDependency> GetDependency<TDependency>();

        /// <summary>
        /// Prepaire a function to be called
        /// </summary>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <typeparam name="TType">Type of object to call the function on.</typeparam>
        /// <param name="methodName">Name of the function.</param>
        /// <param name="propertyValues">Values for properties that should not be auto created.</param>
        /// <returns>A function to call a function on the provided object.</returns>
        public Func<TType, TResult> PrepairFunction<TResult, TType>(string methodName, KeyValuePair<string, object>[] propertyValues, Type[] genricTypes)
            where TType : class;
    }
}
