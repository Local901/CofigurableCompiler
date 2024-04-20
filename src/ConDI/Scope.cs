using System;
using System.Collections.Generic;

namespace ConDI
{
    public class Scope : IScopeFactory, IDependencyFactory
    {
        private IReadOnlyDictionary<Type, DependencyProperties> _dependencies;
        private Scope? _parent;
        private Dictionary<Type, Dependency> _instances = new Dictionary<Type, Dependency>();

        public Scope(IReadOnlyDictionary<Type, DependencyProperties> dependencies)
            : this(dependencies, null) { }
        public Scope(IReadOnlyDictionary<Type, DependencyProperties> dependencies, Scope? parent)
        {
            _dependencies = dependencies;
            _parent = parent;
        }

        public TInstance CreateInstance<TInstance>()
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

        public TInstance GetInstance<TInstance>()
        {
            DependencyProperties? dependency = _dependencies.GetValueOrDefault(typeof(TInstance));

            if (dependency == null)
            {
                if (typeof(TInstance).IsClass)
                {
                    
                }
            }

            throw new NotImplementedException();
        }

        public bool IsKnownDependency<TDepenedency>()
        {
            if (_dependencies.ContainsKey(typeof(TDepenedency)))
            {
                return true;
            }
            else if (_parent != null)
            {
                return _parent.IsKnownDependency<TDepenedency>();
            }
            return false;
        }

        public Dependency GetDependency<TDependency>()
        {
            throw new NotImplementedException();
        }
    }
}
