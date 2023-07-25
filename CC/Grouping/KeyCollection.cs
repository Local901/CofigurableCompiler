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
            if (GetRelationOfKey(key.Key) != null)
                throw new KeyAlreadyExistsException(key.Key);

            // add relation
            var r = new Relation(key);
            Relations.Add(r);

            // add relations
            if (key.IsGroup)
            {
                r.Relations = r.Keys.Where(k => k.Key != key.Key)
                    .Select(k => GetRelationOfKey(k.Key))
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

        public IKey GetKey(string key)
        {
            return GetRelationOfKey(key)?.Key;
        }

        /// <summary>
        /// Get the relation with provided key.
        /// </summary>
        /// <param name="key">Key of desired relation.</param>
        /// <returns>The relation if key was found else null.</returns>
        public Relation GetRelationOfKey(string key)
        {
            return Relations.Where(r => r.Key.CompareTo(key) == 0)
                .OrderBy(r => r.Key.GetKeys().Count())
                .FirstOrDefault();
        }

        /// <summary>
        /// Get all keys that have been added to the collection.
        /// </summary>
        /// <returns>List of distinct keys</returns>
        public List<IKey> GetAllKeys()
        {
            return Relations.Select(r => r.Key)
                .ToList();
        }

        /// <summary>
        /// Get all keys related to the key.
        /// </summary>
        /// <param name="key">A single key string.</param>
        /// <returns>List of related member keys.</returns>
        public List<IKey> GetMemeberKeys(string key)
        {
            var r = GetRelationOfKey(key);
            if (r == null) throw new Exceptions.KeyNotFoundException(key);
            return r.Members;
        }
        /// <summary>
        /// Get all keys related to the keys.
        /// </summary>
        /// <param name="keys">A list of keys</param>
        /// <returns>List of distinct related member keys.</returns>
        public List<IKey> GetMemberKeys(IEnumerable<string> keys)
        {
            return keys.SelectMany(k =>
            {
                try
                {
                    return GetMemeberKeys(k);
                }
                catch (Exceptions.KeyNotFoundException)
                {
                    return new List<IKey>();
                }
            })
                .Distinct()
                .ToList();
        }
        /// <summary>
        /// Get all objects of type T related to the key.
        /// </summary>
        /// <typeparam name="T">Type that inherited IKey.</typeparam>
        /// <param name="key">A key string to get member keys.</param>
        /// <returns>List of type T related to the key.</returns>
        public List<T> GetMemberKeysOfType<T>(string key) where T : IKey
        {
            return GetMemeberKeys(key).OfType<T>()
                .Cast<T>()
                .ToList();
        }
        /// <summary>
        /// Get all objects of type T related to the keys.
        /// </summary>
        /// <typeparam name="T">Type that inherited IKey.</typeparam>
        /// <param name="keys">A list of keys to get distinct member keys.</param>
        /// <returns>List of type T related to the keys.</returns>
        public List<T> GetMemberKeysOfType<T>(IEnumerable<string> keys) where T : IKey
        {
            return GetMemberKeys(keys).OfType<T>()
                .ToList();
        }
        /// <summary>
        /// Get all objects of type T.
        /// </summary>
        /// <typeparam name="T">Type that inherited IKey.</typeparam>
        /// <returns>List of type T.</returns>
        public List<T> GetAllKeysOfType<T>() where T : IKey
        {
            return GetAllKeys().OfType<T>()
                .ToList();
        }

        /// <summary>
        /// Check if key is/ is part of a certain groupKey.
        /// </summary>
        /// <param name="key">The string name of the key.</param>
        /// <param name="group">The groupKey/ key string.</param>
        /// <returns>True is key is related to the groep.</returns>
        public bool IsKeyOfGroup(string key, string group)
        {
            return GetMemeberKeys(group)
                .Select(k => k.Key)
                .Contains(key);
        }
        /// <summary>
        /// Check if key is/ is part of a certain groupKey.
        /// </summary>
        /// <param name="key">The key Object.</param>
        /// <param name="group">The groupKey/ key string.</param>
        /// <returns>True is key is related to the groep.</returns>
        public bool IsKeyOfGroup(IKey key, string group)
        {
            return GetMemeberKeys(group).Contains(key);
        }
    }
}
