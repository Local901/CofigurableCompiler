using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ConDI
{
    public abstract class Dependency : IDependency
    {
        public DependencyProperties Properties { get; }
        protected readonly IDependencyFactory Factory;

        public object? Instance
        {
            get {
                if (Properties.DependencyType == DependencyType.Transient) return ((IDependency)this).CreateInstance();
                return _instance ??= ((IDependency)this).CreateInstance();
            }
        }
        private object? _instance;

        protected Dependency(DependencyProperties properties, IDependencyFactory factory)
        {
            Properties = properties;
            Factory = factory;
        }

        object? IDependency.CreateInstance()
        {
            throw new NotImplementedException();
        }
    }

    public class Dependency<T> : Dependency, IDependency<T>
    {
        public T? Instance => (T)base.Instance;

        public Dependency(DependencyProperties properties, IDependencyFactory factory)
            : base(properties, factory)
        { }

        public T? CreateInstance()
        {
            return Factory.CreateInstance<T>();
        }

        object? IDependency.CreateInstance()
        {
            return CreateInstance();
        }
    }
}
