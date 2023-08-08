using CC.Key;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Blocks
{
    public class ErrorBlock : Block
    {
        public IBlock Block { get; }

        public ErrorBlock(IBlock block, IKey key, string name)
            : base(key, null, block.Index, block.EndIndex, name)
        {
            Block = block;
        }

        public ErrorBlock(IBlock block, IKey key)
            : base(key, null, block.Index, block.EndIndex, block.Name)
        {
            Block = block;
        }

        public new ErrorBlock Copy(string name = null)
        {
            return name == null
                ? new ErrorBlock(Block, Key)
                : new ErrorBlock(Block, Key, name);
        }
    }
}
