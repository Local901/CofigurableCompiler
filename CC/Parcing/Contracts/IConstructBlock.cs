using CC.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public interface IConstructBlock : IBlock
    {
        public IConstructBlock Parent { get; }
        public List<IConstructBlock> Content { get; }
    }
}
