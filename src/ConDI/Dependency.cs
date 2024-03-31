using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ConDI
{
    public abstract class Dependency : IDependency<object>
    {
        public DependencyProperties Properties { get; }

        public object Instance
        {
            get {
                return _instance ??= CreateInstance();
            }
        }
        private object? _instance;

        protected Dependency(DependencyProperties properties)
        {
            Properties = properties;
        }

        abstract public object CreateInstance();
    }

    public class Dependency<T> : Dependency, IDependency<T>
    {
        private readonly IDependencyFactory _factory;
        T IDependency<T>.Instance => (T)Instance;

        public Dependency(DependencyProperties properties, IDependencyFactory factory)
            : base(properties)
        {
            _factory = factory;
        }

        T IDependency<T>.CreateInstance()
        {
            return _factory.CreateInstance<T>();
        }

        public override object CreateInstance()
        {
            return ((IDependency<T>)this).CreateInstance();
        }
    }
}
