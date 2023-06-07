using CC.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Grouping
{
    public class KeyCollection
    {
        public string Name { get; set; }
        public List<Relation> Relations { get; }

        public KeyCollection()
            : this("")
        { }
        public KeyCollection(string name)
        {
            Name = name;
            Relations = new List<Relation>();
        }

        /// <summary>
        /// Add this as a relation.
        /// </summary>
        /// <param name="key"></param>
        public void AddKey(IKey key)
        {
            // No relations with key
            if (GetRelation(key.Key) != null)
                throw new KeyAlreadyExistsException(key.Key);

            // add relation
            var r = new Relation(key);
            Relations.Add(r);

            // add relations
            if (key.IsGroup)
            {
                r.Relations = r.Keys.Select(k => GetRelation(k.Key))
                    .Where(r => r != null)
                    .ToList();
            }

            // add this to other relations
            Relations.Where(r => r.Key.IsGroup)
                .Where(r => r.Keys.Contains(key))
                .ToList()
                .ForEach(re =>
                {
                    re.Relations.Add(r);
                });
        }
        /// <summary>
        /// Reload rlated relations of all relations.
        /// </summary>
        public void ReloadRelations()
        {
            Relations.ForEach(re =>
            {
                if (re.Key.IsGroup)
                {
                    re.Relations = re.Keys.Select(k => GetRelation(k.Key))
                        .Where(r => r != null)
                        .ToList();
                }
                else
                {
                    re.Relations = new List<Relation>();
                }
            });
        }

        /// <summary>
        /// Get the relation with provided key.
        /// </summary>
        /// <param name="key">Key of desired relation.</param>
        /// <returns>The relation if key was found else null.</returns>
        public Relation GetRelation(string key)
        {
            return Relations.FirstOrDefault(r => r.Key.Key.CompareTo(key) == 0);
        }

        /// <summary>
        /// Get all keys related to the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>List of relavant keys.</returns>
        public List<IKey> GetKeys(string key)
        {
            var r = Relations.FirstOrDefault(r => r.Key.Key.CompareTo(key) == 0);
            if (r == null) throw new Exceptions.KeyNotFoundException(key);
            return r.Members;
        }
        /// <summary>
        /// Get all keys related to the keys.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>List of distinct relavant keys.</returns>
        public List<IKey> GetKeys(IEnumerable<string> keys)
        {
            return keys.Select(k =>
            {
                try
                {
                    return GetKeys(k);
                }
                catch (Exceptions.KeyNotFoundException)
                {
                    return new List<IKey>();
                }
            })
                .Aggregate((l1, l2) =>
                {
                    l1.AddRange(l2);
                    return l1;
                })
                .Distinct()
                .ToList();
        }
        /// <summary>
        /// Get all keys.
        /// </summary>
        /// <returns>List of distinct keys</returns>
        public List<IKey> GetAll()
        {
            return Relations.Select(r => r.Members)
                .Aggregate((l1, l2) =>
                {
                    l1.AddRange(l2);
                    return l1;
                })
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Get all objects of type T related to the key.
        /// </summary>
        /// <typeparam name="T">Type that inherited IKey.</typeparam>
        /// <param name="key"></param>
        /// <returns>List of type T related to the key.</returns>
        public List<T> GetKeys<T>(string key) where T : IKey
        {
            return GetKeys(key).OfType<T>()
                .Cast<T>()
                .ToList();
        }
        /// <summary>
        /// Get all objects of type T related to the keys.
        /// </summary>
        /// <typeparam name="T">Type that inherited IKey.</typeparam>
        /// <param name="key"></param>
        /// <returns>List of type T related to the keys.</returns>
        public List<T> GetKeys<T>(IEnumerable<string> keys) where T : IKey
        {
            return GetKeys(keys).OfType<T>()
                .Cast<T>()
                .ToList();
        }
        /// <summary>
        /// Get all objects of type T.
        /// </summary>
        /// <typeparam name="T">Type that inherited IKey.</typeparam>
        /// <returns>List of type T.</returns>
        public List<T> GetAll<T>() where T : IKey
        {
            return GetAll().OfType<T>()
                .Cast<T>()
                .ToList();
        }

        /// <summary>
        /// Check if key is of a certain group.
        /// </summary>
        /// <param name="key">The name og the key.</param>
        /// <param name="group">The group name.</param>
        /// <returns></returns>
        public bool IsKeyOfGroup(string key, string group)
        {
            return GetKeys(group).Any(k => k.Key.CompareTo(key) == 0);
        }
        /// <summary>
        /// Check if key is of a certain group.
        /// </summary>
        /// <param name="key">The key Object.</param>
        /// <param name="group">The group name.</param>
        /// <returns></returns>
        public bool IsKeyOfGroup(IKey key, string group)
        {
            return GetKeys(group).Contains(key);
        }
    }
}
