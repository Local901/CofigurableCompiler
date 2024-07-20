using ConCore.Key;

namespace ConCore.Blocks
{
    public class ErrorBlock : IBlock
    {
        public IBlock Block { get; }

        public IKey Key { get; }

        public string? Name { get; }

        public int Index => Block.Index;

        public int EndIndex => Block.EndIndex;

        public ErrorBlock(IBlock block, IKey key, string? name)
        {
            Block = block;
            Key = key;
            Name = name;
        }

        public ErrorBlock(IBlock block, IKey key)
            : this(block, key, null) { }

        public IBlock Copy(string? name = null)
        {
            return name == null
                ? new ErrorBlock(Block, Key)
                : new ErrorBlock(Block, Key, name);
        }
    }
}
