using ConDI.InstanceFactories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConDI
{
    public interface IInjectorSetup
    {
        /// <summary>
        /// Add a singleton dependency.
        /// 
        /// Singleton dependencies only get created once.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        public void AddSingleton<TImplementation>();
        /// <summary>
        /// Add a singleton dependency for a reference type.
        /// 
        /// Singleton dependencies only get created once.
        /// </summary>
        /// <typeparam name="TReference">The reference type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        public void AddSingleton<TReference, TImplementation>()
            where TImplementation : TReference;
        /// <summary>
        /// Add a singleton dependency for a reference type with a factory function.
        /// 
        /// Singleton dependencies only get created once.
        /// </summary>
        /// <param name="generator">Generator function for the instance.</param>
        /// <typeparam name="TReference">The reference type.</typeparam>
        public void AddSingleton<TReference>(InstanceGenerator<TReference> generator);


        /// <summary>
        /// Add a scoped dependency.
        /// 
        /// Scoped dependencies are unique inside a scope.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        public void AddScoped<TImplementation>();
        /// <summary>
        /// Add a scoped dependency for a reference type.
        /// 
        /// Scoped dependencies are unique inside a scope.
        /// </summary>
        /// <typeparam name="TReference">The refenrence type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        public void AddScoped<TReference, TImplementation>()
            where TImplementation : TReference;
        /// <summary>
        /// Add a scoped dependency for a reference type with a factory function.
        /// 
        /// Scoped dependencies are unique inside a scope.
        /// </summary>
        /// <param name="generator">Generator function for the instance.</param>
        /// <typeparam name="TReference">The refenrence type.</typeparam>
        public void AddScoped<TReference>(InstanceGenerator<TReference> generator);


        /// <summary>
        /// Add a trancient dependency.
        /// 
        /// Trancients get recreated every time it is needed.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        public void AddTrancient<TImplementation>();
        /// <summary>
        /// Add a trancient dependency for a reference type.
        /// 
        /// Trancients get recreated every time it is needed.
        /// </summary>
        /// <typeparam name="TReference">The reference type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        public void AddTrancient<TReference, TImplementation>()
            where TImplementation : TReference;
        /// <summary>
        /// Add a trancient dependency for a reference type with a factory function.
        /// 
        /// Trancients get recreated every time it is needed.
        /// </summary>
        /// <param name="generator">Generator function for the instance.</param>
        /// <typeparam name="TReference">The reference type.</typeparam>
        public void AddTrancient<TReference>(InstanceGenerator<TReference> generator);
    }
}
