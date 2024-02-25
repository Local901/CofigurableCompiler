using ConCore.Blocks;

namespace ConCore.Parsing.Simple.Contracts
{
    public interface IParseCompletion
    {
        public int Round { get; }
        public IBlock Block { get; }
    }
}
