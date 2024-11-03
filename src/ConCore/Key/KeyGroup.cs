using System.Collections.Generic;
using System.Linq;

namespace ConCore.Key
{
    public class KeyGroup : IKey
    {
        private List<KeyLangReference> _members;
        public IReadOnlyList<KeyLangReference> Members { get => _members; }

        public KeyGroup(string key)
            : this(key, new List<KeyLangReference>())
        { }
        public KeyGroup(string key, List<KeyLangReference> members)
            : base(key)
        {
            _members = members;
        }

        public void Add(KeyLangReference keyReference)
        {
            _members.Add(keyReference);
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
