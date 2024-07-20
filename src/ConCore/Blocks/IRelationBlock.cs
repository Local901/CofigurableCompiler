using System.Collections.Generic;

namespace ConCore.Blocks
{
    public interface IRelationBlock : IBlock
    {
        public IRelationBlock? Parent { get; set; }
        public IReadOnlyList<IBlock> Content { get; }
    }
}
