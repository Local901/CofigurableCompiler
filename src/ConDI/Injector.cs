using ConDI.InstanceFactories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConDI
{
    public class Injector : IInjectorSetup, IScopeFactory
    {
        private Dictionary<Type, DependencyProperties> Dependencies = new Dictionary<Type, DependencyProperties>();

        public void AddScoped<TImplementation>()
        {
            AddScoped<TImplementation, TImplementation>();
        }
        public void AddScoped<TReference, TImplementation>()
            where TImplementation : TReference
        {
            Dependencies.Add(typeof(TReference), new DependencyProperties(InstanceScope.Scoped, new ScopedFactory<TImplementation>()));
        }
        public void AddScoped<TReference>(InstanceGenerator<TReference> generator)
        {
            Dependencies.Add(typeof(TReference), new DependencyProperties(InstanceScope.Scoped, new FunctionFactory<TReference>(generator)));
        }

        public void AddSingleton<TImplementation>()
        {
            AddSingleton<TImplementation, TImplementation>();
        }
        public void AddSingleton<TReference, TImplementation>()
            where TImplementation : TReference
        {
            Dependencies.Add(typeof(TReference), new DependencyProperties(InstanceScope.Singleton, new ScopedFactory<TImplementation>()));
        }
        public void AddSingleton<TReference>(InstanceGenerator<TReference> generator)
        {
            Dependencies.Add(typeof(TReference), new DependencyProperties(InstanceScope.Singleton, new FunctionFactory<TReference>(generator)));
        }

        public void AddTrancient<TImplementation>()
        {
            AddTrancient<TImplementation, TImplementation>();
        }
        public void AddTrancient<TReference, TImplementation>()
            where TImplementation : TReference
        {
            Dependencies.Add(typeof(TReference), new DependencyProperties(InstanceScope.Transient, new ScopedFactory<TImplementation>()));
        }
        public void AddTrancient<TReference>(InstanceGenerator<TReference> generator)
        {
            Dependencies.Add(typeof(TReference), new DependencyProperties(InstanceScope.Transient, new FunctionFactory<TReference>(generator)));
        }

        /// <summary>
        /// Create a fresh scope without created singleton references.
        /// </summary>
        /// <returns></returns>
        public Scope CreateScope()
        {
            return new Scope(Dependencies);
        }

        public Scope CreateScope(Scope parent)
        {
            return new Scope(Dependencies, parent);
        }
    }
}
