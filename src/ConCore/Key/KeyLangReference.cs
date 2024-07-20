using ConCore.Key.Collections;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ConCore.Key
{
    public class KeyLangReference : IComparable<KeyLangReference>
    {
        public LangCollection Language { get; internal set; }
        public string Key { get; internal set; }
        public string Lang { get => Language.Language; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is not KeyLangReference) return false;
            return Key == ((KeyLangReference)obj).Key && Lang == ((KeyLangReference)obj).Lang;
        }

        public override string ToString()
        {
            return $"{Lang}:{Key}";
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
    }
}
