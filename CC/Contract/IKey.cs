using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace CC.Contract
{
    public abstract class IKey : IComparable, IComparable<string>, IComparable<IKey>
    {
        public string Key { get; set; }

        /// <summary>
        /// Get a subkey related to value if applicable
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Found key</returns>
        public abstract IKey GetKeyFor(string value);

        /// <summary>
        /// Get all child keys related to this key (this included).
        /// </summary>
        /// <returns>List of keys</returns>
        public abstract List<IKey> GetKeys();

        /// <summary>
        /// Are subtoken more specific versions of this key?
        /// </summary>
        public bool IsGroup { get; set; }

        public override string ToString()
        {
            return Key;
        }

        public override bool Equals(object obj)
        {
            if (obj is string) return Key == (string)obj;
            if (obj is IKey) return Key == ((IKey)obj).Key;
            return false;
        }

        public int CompareTo(object other)
        {
            if (other == null) return -1;
            if (!(other is IKey)) return -1;

            return Key.CompareTo(((IKey)other).Key);
        }

        public int CompareTo([AllowNull] string other)
        {
            return GetKeys().Select(k => k.Key.CompareTo(other))
                .OrderBy(o => Math.Abs(o))
                .First();
        }

        public int CompareTo([AllowNull] IKey other)
        {
            return CompareTo(other?.Key);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key);
        }
    }
}
