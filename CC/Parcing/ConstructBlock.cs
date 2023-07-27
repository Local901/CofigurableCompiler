using BranchList;
using CC.Contract;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Parcing
{
    public class ConstructBlock : Block
    {
        public ConstructBlock Parent { get; protected set; }
        public IReadOnlyList<ConstructBlock> Content { get; protected set; }

        private ConstructBlock() { }
        public ConstructBlock(IConstruct key, IEnumerable<IBlock> content)
        {
            if (content.Count() == 0) throw new ArgumentException("The content for a construct should at least contain one element.");

            Content = content.Select(b =>
            {
                if (b is ConstructBlock) return b as ConstructBlock;
                return new ConstructBlock
                {
                    Key = b.Key,
                    Name = b.Name,
                    Value = b.Value,
                    Parent = this,
                    Index = b.Index,
                    EndIndex = b.EndIndex
                };
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
            copy.Content.ForEach(c => c.Parent = copy);

            return copy;
        }
    }
}
