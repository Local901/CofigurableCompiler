using CC.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Key.Modifiers
{
    public abstract class IModifier : IKey
    {
        public readonly IBlockReader BlockReader;

        public IModifier(IBlockReader blockReader)
        {
            BlockReader = blockReader;
        }

        public abstract IBlock[] FindBlocks(IBlock block);
    }
}
