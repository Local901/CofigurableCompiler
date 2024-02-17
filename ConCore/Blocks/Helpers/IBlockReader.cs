using System;

namespace ConCore.Blocks.Helpers
{
    public interface IBlockReader
    {
        /// <summary>
        /// Select all block that cerespond to the predicate.
        /// </summary>
        /// <param name="block">The block to search.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>An array of blocks that match the predicate.</returns>
        IBlock[] SelectAll(IBlock block, Predicate<IBlock> predicate);
        /// <summary>
        /// Select all block that cerespond to the predicate.
        /// </summary>
        /// <typeparam name="TBlock">The type of the block to search.</typeparam>
        /// <param name="block">The block to search.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>An array of blocks that match the predicate.</returns>
        TBlock[] SelectAll<TBlock>(IBlock block, Predicate<TBlock> predicate) where TBlock : IBlock;

        /// <summary>
        /// Try to traverse the tree of blocks.
        /// </summary>
        /// <param name="block">The root to search from.</param>
        /// <param name="path">The pth to follow.</param>
        /// <returns>The block that was found. Else it is null.</returns>
        IBlock TraverseBlock(IBlock block, string[] path);
    }
}
