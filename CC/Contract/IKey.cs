using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CC.Contract
{
    public abstract class IKey : IComparable
    {
        public string Key { get; set; }

        /// <summary>
        /// Get a subkey related to value if applicable
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Found key</returns>
        public abstract IKey GetKey(object value);

        /// <summary>
        /// Get all subkeys.
        /// </summary>
        /// <returns>List of subkeys</returns>
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
            return CompareTo(obj) == 0;
        }

        public int CompareTo(object other)
        {
            if (other == null) return -1;
            if (!(other is IKey)) return -1;

            return Key.CompareTo(((IKey)other).Key);
        }
    }
}
