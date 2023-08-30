using BranchList;
using CC.Key;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Blocks
{
    public class ConstructBlock : Block, IRelationBlock
    {
        public IRelationBlock Parent { get; set; }
        public IReadOnlyList<IBlock> Content { get; protected set; }

        private ConstructBlock() { }
        public ConstructBlock(IConstruct key, IEnumerable<IBlock> content)
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
            Index = Content.First().Index;
            EndIndex = Content.Last().EndIndex;
        }

        public new ConstructBlock Copy(string name = null)
        {
            ConstructBlock copy = new ConstructBlock
            {
                Key = Key,
                Name = name == null ? Name : name,
                Value = Value,
                Index = Index,
                EndIndex = EndIndex,
                Content = Content.Select(c => c.Copy()).ToList()
            };
            // set Parent to be the copy for all children.
            copy.Content.ForEach(c => 
            {
                if (c is IRelationBlock) {
                    ((IRelationBlock)c).Parent = copy;
                }
            });

            return copy;
        }
    }
}
