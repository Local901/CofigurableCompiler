using CC.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public interface IConstructParsingArgs
    {
        public IConstruct Key { get; }
        public IReadOnlyList<IBlock> Content { get; }

        public IConstructParseResult TryUseBlock(IBlock block);
        public IBlock MakeBlock();
    }
}
