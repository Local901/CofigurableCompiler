using CC.Contract;
using CC.Parcing.ComponentTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public interface IParseFactory
    {
        public ConstructBlock LastCompletion { get; }
        public List<ValueComponent> GetNextKeys();
        public void UseBlock(IBlock block);
    }
}
