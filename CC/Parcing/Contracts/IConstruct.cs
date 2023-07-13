using CC.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public abstract class IConstruct : IKey
    {
        public IComponent Components { get; }

        public IConstruct (IComponent components)
        {
            Components = components;
        }
    }
}
