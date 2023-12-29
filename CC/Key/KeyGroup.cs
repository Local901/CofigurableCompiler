using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Key
{
    public class KeyGroup : IKey
    {
        public IList<KeyLangReference> Members { get; }

        public KeyGroup(string key)
            : this(key, new List<KeyLangReference>())
        { }
        public KeyGroup(string key, IList<KeyLangReference> members)
        {
            Reference = new KeyLangReference
            {
                Key = key,
            };
            Members = members;
        }

        /// <summary>
        /// Get all child keys related to this key (this included).
        /// </summary>
        /// <returns>List of keys</returns>
        public List<KeyLangReference> GetSubKeyRefs()
        {
            return Members.ToList();
        }
    }
}
