using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public interface IComponent
    {
        public string Key { get; }
        public string Name { get; }
    }
}
