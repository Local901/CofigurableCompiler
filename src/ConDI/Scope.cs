using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace ConDI
{
    public class Scope : IScopeFactory, IDependencyFactory
    {
        private readonly IReadOnlyDictionary<Type, DependencyProperties> Dependencies;
        private readonly Scope? Parent;
        private readonly Dictionary<Type, Dependency> Instances = new();

        public Scope(IReadOnlyDictionary<Type, DependencyProperties> dependencies)
            : this(dependencies, null) { }
        public Scope(IReadOnlyDictionary<Type, DependencyProperties> dependencies, Scope? parent)
        {
            Dependencies = dependencies;
            Parent = parent;
        }

        public TInstance? CreateInstance<TInstance>()
        {
            throw new NotImplementedException();
        }

        public Scope CreateScope()
        {
            return new Scope(Dependencies, this);
        }

        public Scope CreateScope(Scope parent)
        {
            return new Scope(Dependencies, parent);
        }

        public TInstance? GetInstance<TInstance>()
        {
            try
            {
                Dependency<TInstance> dependency = GetDependency<TInstance>();
                return dependency.CreateInstance();
            }
            catch (Exception)
            {
                return CreateInstance<TInstance>();
            }
        }

        public bool IsKnownDependency<TDepenedency>()
        {
            if (Dependencies.ContainsKey(typeof(TDepenedency)))
            {
                return true;
            }
            else if (Parent != null)
            {
                return Parent.IsKnownDependency<TDepenedency>();
            }
            return false;
        }

        public Dependency<TDependency> GetDependency<TDependency>()
        {
            DependencyProperties? dependencyProp = Dependencies.GetValueOrDefault(typeof(TDependency));

            // Test if the dependency exists in this scope.
            if (dependencyProp == null)
            {
                if (Parent == null)
                {   // At this point the dependency doesn't exists in any scope.
                    throw new Exception($"No dependence exists of type {nameof(TDependency)}");
                }
                // Ask parent scope if it knows the dependency.
                return Parent.GetDependency<TDependency>();
            }

            Dependency? dependency = Instances.GetValueOrDefault(typeof(TDependency));

            // If the dependency already exists in the dictionary it means it has been deemd to be correct before.
            if (dependency != null && dependency is Dependency<TDependency> result) return result;

            // Check Singlons with hte parent becaese parent goes before scope value.
            if (((DependencyProperties)dependencyProp).DependencyType == DependencyType.Singleton)
            {
                dependency = Parent?.GetDependency<TDependency>();
                var r = dependency as Dependency<TDependency>;
                if (dependency != null && r != null) return r;
            }

            // Make dependency in scope because it should exixt.
            // Scoped and Transient dependencies are always made when they don't exist.
            result = new Dependency<TDependency>((DependencyProperties)dependencyProp, this);
            Instances.Add(typeof(TDependency), result);
            return result;
        }
    }
}
