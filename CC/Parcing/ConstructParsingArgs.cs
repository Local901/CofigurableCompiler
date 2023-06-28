using CC.Contract;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Parcing
{
    internal class ConstructParsingArgs : IConstructParsingArgs
    {
        private readonly IConstruct _key;
        private readonly List<IBlock> _content;

        public IConstruct Key => _key;

        public IReadOnlyList<IBlock> Content => _content;

        public ConstructParsingArgs(IConstruct Key)
        {
            _key = Key;
            _content = new List<IBlock>();
        }
        private ConstructParsingArgs(IConstruct Key, List<IBlock> Content)
        {
            _key = Key;
            _content = Content;
        }

        public IBlock MakeBlock()
        {
            if (_content.Count == 0) return null;

            return new Block
            {
                Key = _key,
                Content = _content,
                Index = _content[0].Index,
                EndIndex = _content.Last().EndIndex,
            };
        }

        public IConstructParseResult TryUseBlock(IBlock block)
        {
            throw new NotImplementedException();
        }

        public IConstructParsingArgs Copy()
        {
            return new ConstructParsingArgs(_key, _content);
        }
    }
}
