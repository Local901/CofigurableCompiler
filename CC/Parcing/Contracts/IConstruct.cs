using CC.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public abstract class IConstruct : IKey
    {
        public IReadOnlyList<IComponent> Components { get; }
    }
}
