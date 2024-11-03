using ConCore.Key;

namespace ConCore.Blocks
{
    public interface IBlock
    {
        IKey? Key { get; }
        string? Name { get; }
        CharacterPosition Index { get; }
        CharacterPosition EndIndex { get; }

        IBlock Copy(string? name = null);
    }
}
