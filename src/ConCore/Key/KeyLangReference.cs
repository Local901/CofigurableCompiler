using ConCore.Key.Collections;
using System;

namespace ConCore.Key
{
    public class KeyLangReference : IComparable<KeyLangReference>
    {
        public Language? Language { get; }
        public string Key { get; }
        public string? Lang { get => Language?.Name; }

        public KeyLangReference(string key)
            : this(null, key) { }

        public KeyLangReference(Language? language, string key)
        {
            Language = language;
            Key = key;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is not KeyLangReference other) return false;
            return Lang == other.Lang && Key == other.Key;
        }

        public override string ToString()
        {
            return $"{Lang}::{Key}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Lang);
        }

        public int CompareTo(KeyLangReference? other)
        {
            int index = string.Compare(Lang, other?.Lang);
            if (index != 0) return index;
            return string.Compare(Key, other?.Key);
        }

        public static bool operator ==(KeyLangReference? left, KeyLangReference? right)
        {
            if (Object.ReferenceEquals(left, null))
            {
                if (Object.ReferenceEquals(right, null))
                {
                    return true;
                }
                return false;
            }
            return left.Equals(right);
        }
        public static bool operator !=(KeyLangReference? left, KeyLangReference? right)
        {
            return !(left == right);
        }
    }
}
