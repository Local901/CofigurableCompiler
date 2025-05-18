using ConCore.Key;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConCore.Blocks
{
    public class ConstructBlock : IRelationBlock
    {
        public IKey Key { get; private set; }

        public string? Name { get; private set; }

        public CharacterPosition Index { get; private set; }

        public CharacterPosition EndIndex { get; private set; }
        public IRelationBlock? Parent { get; set; }
        public IReadOnlyList<IBlock> Content { get; protected set; }

        public ConstructBlock(Construct key, IEnumerable<IBlock> content)
        {
            if (content.Count() == 0) throw new ArgumentException("The content for a construct should at least contain one element.");

            Content = content.Select(b =>
            {
                var relationBlock = b as IRelationBlock;
                if (relationBlock == null) return b;
                relationBlock.Parent = this;
                return relationBlock;
            }).ToList();

            Key = key;
            Index = Content[0].Index;
            EndIndex = Content[Content.Count - 1].EndIndex;
        }

        public ConstructBlock Copy(string? name = null)
        {
            ConstructBlock copy = new((Construct)Key, Content.Select((c) => c.Copy()))
            {
                Name = name ?? Name
            };

            // set Parent to be the copy for all children.
            foreach(var c in copy.Content)
            {
                if (c is IRelationBlock relationBlock) {
                    relationBlock.Parent = copy;
                }
            }

            return copy;
        }

        IBlock IBlock.Copy(string? name)
        {
            return Copy(name);
        }
    }
}
