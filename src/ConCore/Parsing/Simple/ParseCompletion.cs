using ConCore.Blocks;
using ConCore.Parsing.Simple.Contracts;

namespace ConCore.Parsing.Simple
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
