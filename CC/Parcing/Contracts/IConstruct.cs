using CC.Contract;
using CC.Parcing.ComponentTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public abstract class IConstruct : IKey
    {
        public IComponent Components { get; }

        public IConstruct (string key, IComponent components)
        {
            Key = key;
            Components = components;
        }
    }
}
