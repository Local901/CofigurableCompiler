using CC.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC
{
    public class Block : IBlock
    {
        public IKey Key { get; protected set; }
        public string Name { get; protected set; }
        public string Value { get; protected set; }
        public int Index { get; protected set; }
        public int EndIndex { get; protected set; }

        protected Block() { }
        public Block(IKey key, string value, int index, int endIndex, string name = null)
        {
            Key = key;
            Name = name;
            Value = value;
            Index = index;
            EndIndex = endIndex;
        }

        public virtual IBlock Copy(string name = null)
        {
            return new Block
            {
                Key = Key,
                Name = name == null ? Name : name,
                Value = Value,
                Index = Index,
                EndIndex = EndIndex
            };
        }
    }
}
