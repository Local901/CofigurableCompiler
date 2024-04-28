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
            Dependencies.Add(typeof(TReference), new DependencyProperties(typeof(TReference), DependencyType.Scoped));
        }

        public void AddSingleton<TImplementation>()
        {
            AddSingleton<TImplementation, TImplementation>();
        }
        public void AddSingleton<TReference, TImplementation>()
            where TImplementation : TReference
        {
            Dependencies.Add(typeof(TReference), new DependencyProperties(typeof(TReference), DependencyType.Singleton));
        }

        public void AddTrancient<TImplementation>()
        {
            AddTrancient<TImplementation, TImplementation>();
        }
        public void AddTrancient<TReference, TImplementation>()
            where TImplementation : TReference
        {
            Dependencies.Add(typeof(TReference), new DependencyProperties(typeof(TReference), DependencyType.Transient));
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
