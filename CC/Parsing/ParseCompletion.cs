using CC.Blocks;
using CC.Parsing.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parsing
{
    public class ParseCompletion : IParseCompletion
    {
        public int Round { get; }
        public IBlock Block { get; }

        public ParseCompletion(int round, IBlock block)
        {
            Round = round;
            Block = block;
        }
    }
}
