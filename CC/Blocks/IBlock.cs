using ConCore.Key;

namespace ConCore.Blocks
{
    public interface IBlock
    {
        IKey Key { get; }
        string Name { get; }
        int Index { get; }
        int EndIndex { get; }

        IBlock Copy(string name = null);
    }
}
