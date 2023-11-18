using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Blocks
{
    public class BlockReader : IBlockReader
    {
        public IBlock[] SelectAll(IBlock block, Predicate<IBlock> predicate)
        {
            return SelectAll<IBlock>(block, predicate);
        }

        public TBlock[] SelectAll<TBlock>(IBlock block, Predicate<TBlock> predicate) where TBlock : IBlock
        {
            var newBlocks = new List<IBlock> { block };
            var result = new List<TBlock>();

            while(newBlocks.Count != 0)
            {
                result.AddRange(
                    newBlocks.OfType<TBlock>()
                        .Where((b) => predicate(b))
                );

                newBlocks = newBlocks.SelectMany((b) =>
                {
                    if (b is IRelationBlock)
                    {
                        return (b as IRelationBlock).Content;
                    }
                    return Array.Empty<IBlock>();
                }).ToList();
            }

            return result.ToArray();
        }
    }
}
