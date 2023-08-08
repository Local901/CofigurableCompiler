using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CC.Key
{
    public class KeyLangReference : IComparable<KeyLangReference>
    {
        public string Key { get; internal set; }
        public string Lang { get; internal set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is KeyLangReference)) return false;
            return Key == ((KeyLangReference)obj).Key && Lang == ((KeyLangReference)obj).Lang;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Lang);
        }

        public int CompareTo([AllowNull] KeyLangReference other)
        {
            int index = string.Compare(Lang, other.Lang);
            if (index != 0) return index;
            return string.Compare(Key, other.Key);
        }
    }
}
