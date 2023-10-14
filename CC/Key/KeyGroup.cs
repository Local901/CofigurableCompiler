using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Key
{
    public class KeyGroup : IKey
    {
        public IReadOnlyList<KeyLangReference> Members { get; }

        public KeyGroup()
        {
            Members = new List<KeyLangReference>();
        }
        public KeyGroup(IReadOnlyList<KeyLangReference> members)
        {
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
