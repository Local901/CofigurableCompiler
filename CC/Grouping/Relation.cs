using CC.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Grouping
{
    public class Relation
    {
        public IKey Key { get; }

        /// <summary>
        /// Returns all keys of this and corolated relatations.
        /// </summary>
        public List<IKey> Members
        {
            get
            {
                if (Relations.Count == 0)
                    return new List<IKey>() { Key };

                var l = Relations.SelectMany(r => r.Members)
                    .ToList();
                l.Add(Key);
                return l;
            }
        }
        /// <summary>
        /// Child relations.
        /// </summary>
        public List<Relation> Relations { get; set; }
        /// <summary>
        /// Child keys of Key.
        /// </summary>
        public List<IKey> Keys
        {
            get => Key.GetKeys();
        }

        public Relation(IKey key)
        {
            Key = key;
            Relations = new List<Relation>();
        }
    }
}
