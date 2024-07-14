using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

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

        public Scope CreateScope()
        {
            return new Scope(Dependencies, this);
        }

        public Scope CreateScope(Scope parent)
        {
            return new Scope(Dependencies, parent);
        }

        public TInstance? CreateInstance<TInstance>()
        {
            return CreateInstance<TInstance>(new KeyValuePair<string, object>[0]);
        }

        public TInstance? CreateInstance<TInstance>(KeyValuePair<string, object>[] propertyValues)
        {
            var type = typeof(TInstance);
            foreach (var constructor in type.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                try
                {
                    var parms = parameters.Select((parameter) =>
                    {
                        return propertyValues.FirstOrDefault((p) => p.Key == parameter.Name).Value
                            ?? GetInstance(parameter.ParameterType);
                    }).ToArray();

                    var instance = constructor.Invoke(parms);
                    if (instance is TInstance instance1)
                    {
                        return instance1;
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return default(TInstance);
        }

        public object? CreateInstance(Type type)
        {
            return typeof(Scope)
                .GetMethod("CreateInstance")
                ?.MakeGenericMethod(type)
                .Invoke(this, null);
        }

        public object? CreateInstance(Type type, KeyValuePair<string, object>[] propertyValues)
        {
            return typeof(Scope)
                .GetMethod("CreateInstance")
                ?.MakeGenericMethod(type)
                .Invoke(this, new object[] { propertyValues });
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

        public object? GetInstance(Type type)
        {
            return typeof(Scope)
                .GetMethod("GetInstance")
                ?.MakeGenericMethod(type)
                .Invoke(this, null);
        }

        public bool IsKnownDependency<TDepenedency>()
        {
            return IsKnownDependency(typeof(TDepenedency));
        }

        public bool IsKnownDependency(Type type)
        {
            if (Dependencies.ContainsKey(type))
            {
                return true;
            }
            else if (Parent != null)
            {
                return Parent.IsKnownDependency(type);
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

        public Func<TType, TResult> PrepairFunction<TResult, TType>(string methodName, KeyValuePair<string, object>[] propertyValues, Type[]? genricTypes = null)
            where TType: new()
        {
            var objectType = typeof(TType);
            var method = objectType.GetMethod(methodName) ?? throw new Exception($"Method {methodName}");
            if (!method.ReturnType.IsAssignableTo(typeof(TResult)))
            {
                throw new Exception($"Return type {method.ReturnType.FullName} can't be assigned to {nameof(TResult)}.");
            }

            object?[]? parameters = method.GetParameters().Select((parameter) =>
                propertyValues.FirstOrDefault((v) => v.Key == parameter.Name).Value
                    ?? GetInstance(parameter.ParameterType)
            ).ToArray();
            
            return genricTypes == null
                ? (TType obj) => (TResult)method.Invoke(obj, parameters)
                : (TType obj) => (TResult)method.MakeGenericMethod(genricTypes).Invoke(obj, parameters);
        }
    }
}
