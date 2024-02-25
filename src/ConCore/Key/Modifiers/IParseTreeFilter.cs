using ConCore.Blocks;
using ConCore.Blocks.Helpers;

namespace ConCore.Key.Modifiers
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
