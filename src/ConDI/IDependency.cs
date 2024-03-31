using System;
using System.Collections.Generic;
using System.Text;

namespace ConDI
{
    public interface IDependency<T>
    {
        public DependencyProperties Properties { get; }
        public T Instance { get; }
        public T CreateInstance();
    }
}
