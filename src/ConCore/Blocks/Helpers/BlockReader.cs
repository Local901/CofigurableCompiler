using System;
using System.Collections.Generic;
using System.Linq;

namespace ConCore.Blocks.Helpers
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

            while (newBlocks.Count != 0)
            {
                result.AddRange(
                    newBlocks.OfType<TBlock>()
                        .Where((b) => predicate(b))
                );

                newBlocks = newBlocks.SelectMany((b) =>
                {
                    if (b is IRelationBlock relationBlock)
                    {
                        return relationBlock.Content;
                    }
                    return Array.Empty<IBlock>();
                }).ToList();
            }

            return result.ToArray();
        }

        public IBlock? TraverseBlock(IBlock block, string[] path)
        {
            var result = block;

            for (int i = 0; i < path.Count() && result != null; i++)
            {
                var step = path[i];

                switch (step)
                {
                    case ".":
                        break;
                    case "..":
                        if (result is IRelationBlock)
                        {
                            result = ((IRelationBlock)result).Parent;
                        }
                        else
                        {
                            result = null;
                        }
                        break;
                    default:
                        if (result is IRelationBlock)
                        {
                            result = ((IRelationBlock)result)
                                .Content.FirstOrDefault((b) => b.Name == step);
                            break; // TODO: Add support for multiple hits.
                        }
                        result = null;
                        break;
                }
            }

            return result;
        }
    }
}
