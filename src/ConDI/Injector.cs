using System;
using System.Collections.Generic;
using System.Text;

namespace ConDI
{
    public class Injector : IInjectorSetup, IScopeFactory
    {
        private Dictionary<Type, Dependency> Dependencies = new Dictionary<Type, Dependency>();

        public void AddScoped<TImplementation>()
        {
            AddScoped<TImplementation, TImplementation>();
        }
        public void AddScoped<TReference, TImplementation>()
            where TImplementation : TReference
        {
            throw new NotImplementedException();
        }

        public void AddSingleton<TImplementation>()
        {
            AddSingleton<TImplementation, TImplementation>();
        }
        public void AddSingleton<TReference, TImplementation>()
            where TImplementation : TReference
        {
            throw new NotImplementedException();
        }

        public void AddTrancient<TImplementation>()
        {
            AddTrancient<TImplementation, TImplementation>();
        }
        public void AddTrancient<TReference, TImplementation>()
            where TImplementation : TReference
        {
            throw new NotImplementedException();
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
