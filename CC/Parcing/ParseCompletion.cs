using CC.Blocks;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing
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
