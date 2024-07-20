using System;
using System.Diagnostics.CodeAnalysis;

namespace ConCore.Key
{
    public abstract class IKey : IComparable, IComparable<KeyLangReference>, IComparable<IKey>
    {
        public KeyLangReference Reference { get; protected set; }

        public override string ToString()
        {
            return $"{Reference}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is KeyLangReference reference) return Reference == reference;
            if (obj is IKey key) return Reference == key.Reference;
            return false;
        }

        public int CompareTo(object? other)
        {
            if (other == null) return -1;
            if (other is KeyLangReference) return CompareTo((KeyLangReference)other);
            if (other is not IKey) return -1;

            return CompareTo(((IKey)other).Reference);
        }

        public int CompareTo(KeyLangReference? other)
        {
            int diff = string.Compare(Reference.Lang, other?.Lang);
            if (diff == 0) return string.Compare(Reference.Key, other?.Key);
            return diff;
        }

        public int CompareTo(IKey? other)
        {
            return CompareTo(other?.Reference);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Reference);
        }
    }
}
