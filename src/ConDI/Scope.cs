using ConDI.Exceptions;
using System;
using System.Collections.Generic;

namespace ConDI
{
    public class Scope : IScopeFactory, IDependencyFactory
    {
        private IReadOnlyDictionary<Type, Dependency> _dependencies;
        private Scope _parent;
        private Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        public Scope(IReadOnlyDictionary<Type, Dependency> dependencies)
            : this(dependencies, null) { }
        public Scope(IReadOnlyDictionary<Type, Dependency> dependencies, Scope parent)
        {
            _dependencies = dependencies;
            _parent = parent;
        }

        public TClass CreateClass<TClass>()
            where TClass : class
        {
            throw new NotImplementedException();
        }

        public Scope CreateScope()
        {
            return new Scope(_dependencies, this);
        }

        public Scope CreateScope(Scope parent)
        {
            return new Scope(_dependencies, parent);
        }

        public TStruct CreateStruct<TStruct>()
            where TStruct : struct
        {
            throw new NotImplementedException();
        }

        public TInstance GetInstance<TInstance>()
        {
            Dependency? dependency = _dependencies.GetValueOrDefault(typeof(TInstance));

            if (dependency == null)
            {
                if (typeof(TInstance).IsClass)
                {
                    return CreateClass
                }
            }

            throw new NotImplementedException();
        }
    }
}
