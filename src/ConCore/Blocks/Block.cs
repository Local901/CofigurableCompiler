using ConCore.Key;

namespace ConCore.Blocks
{
    public class Block : IValueBlock
    {
        public IKey Key { get; protected set; }
        public string? Name { get; protected set; }
        public string Value { get; protected set; }
        public CharacterPosition Index { get; protected set; }
        public CharacterPosition EndIndex { get; protected set; }

        public Block(IKey key, string value, CharacterPosition index, CharacterPosition endIndex, string? name = null)
        {
            Key = key;
            Name = name;
            Value = value;
            Index = index;
            EndIndex = endIndex;
        }

        public virtual IBlock Copy(string? name = null)
        {
            return new Block(
                Key,
                Value,
                Index,
                EndIndex,
                name ?? Name
            );
        }
    }
}
