using BranchList;
using CC.Contract;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Parcing
{
    public class ConstructBlock : BranchNode<ConstructBlock>, IConstructBlock
    {
        public IKey Key { get; private set; }
        public string Name { get; private set; }
        public string Value { get; private set; }
        public int Index {
            get
            {
                if (_index == null)
                    return Children.First().Index;
                return (int)_index;
            }
            private set
            {
                _index = value;
            }
        }
        private int? _index;
        public int EndIndex {
            get
            {
                if (_endIndex == null)
                    return Children.Last().EndIndex;
                return (int)_endIndex;
            }
            private set
            {
                _endIndex = value;
            }
        }
        private int? _endIndex;

        public List<IConstructBlock> Content => Children.Cast<IConstructBlock>().ToList();
        IConstructBlock IConstructBlock.Parent => Parent;

        private ConstructBlock() { }
        public ConstructBlock(IConstruct key, List<IBlock> content)
        {
            Key = key;
            AddRange(content.Select(block =>
            {
                if (block is ConstructBlock) return block as ConstructBlock;
                return new ConstructBlock
                {
                    Key = block.Key,
                    Name = block.Name,
                    Value = block.Value,
                    Index = block.Index,
                    EndIndex = block.EndIndex
                };
            }));
        }
    }
}
