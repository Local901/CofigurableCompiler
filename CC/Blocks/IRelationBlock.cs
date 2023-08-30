using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Blocks
{
    public interface IRelationBlock : IBlock
    {
        public IRelationBlock Parent { get; set; }
        public IReadOnlyList<IBlock> Content { get; }
    }
}
