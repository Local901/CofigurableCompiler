using CC.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Key.Modifiers
{
    /// <summary>
    /// A filter to find blocks in the parsed block tree.
    /// </summary>
    public abstract class IParseTreeFilter : IFilter
    {
        public readonly IBlockReader BlockReader;
        protected IParseTreeFilter(IBlockReader blockReader)
        {
            BlockReader = blockReader;
        }

        public abstract IBlock[] FindBlocks(IBlock block);
    }
}
