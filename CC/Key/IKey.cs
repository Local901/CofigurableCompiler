using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace CC.Key
{
    public abstract class IKey : IComparable, IComparable<KeyLangReference>, IComparable<IKey>
    {
        public KeyLangReference Reference { get; protected set; }

        public override string ToString()
        {
            return $"{Reference}";
        }

        public override bool Equals(object obj)
        {
            if (obj is KeyLangReference) return Reference == (KeyLangReference)obj;
            if (obj is IKey) return Reference == ((IKey)obj).Reference;
            return false;
        }

        public int CompareTo(object other)
        {
            if (other == null) return -1;
            if (other is KeyLangReference) return CompareTo((KeyLangReference)other);
            if (!(other is IKey)) return -1;

            return CompareTo(((IKey)other).Reference);
        }

        public int CompareTo([AllowNull] KeyLangReference other)
        {
            int diff = string.Compare(Reference.Lang, other.Lang);
            if (diff == 0) return string.Compare(Reference.Key, other.Key);
            return diff;
        }

        public int CompareTo([AllowNull] IKey other)
        {
            return CompareTo(other?.Reference);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Reference);
        }
    }
}
