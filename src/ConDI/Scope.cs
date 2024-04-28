using System;
using System.Collections.Generic;
using System.Linq;

namespace ConDI
{
    public class Scope : IScope, IScopeFactory, IDependencyFactory
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
            var type = typeof(TInstance);
            foreach(var constructor in type.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                try
                {
                    var parms = parameters.Select((parameter) =>
                    {
                        return typeof(Scope)
                            .GetMethod("GetInstance")
                            ?.MakeGenericMethod(parameter.ParameterType)
                            .Invoke(this, null);
                    }).ToArray();

                    var instance = constructor.Invoke(parms);
                    if (instance is TInstance instance1)
                    {
                        return instance1;
                    }
                } catch (Exception)
                {
                    continue;
                }
            }
            return default(TInstance);
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
            if (this is TInstance factory)
            {
                return factory;
            }
            try
            {
                Dependency<TInstance> dependency = GetDependency<TInstance>();
                return dependency.Instance;
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
