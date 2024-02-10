using BranchList;
using CC.Key.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Key
{
    public class LangCollection
    {
        public readonly string Language;
        private readonly Dictionary<string, IKey> Keys = new Dictionary<string, IKey>();
        private readonly List<IFilter> Filters = new List<IFilter>();

        public LangCollection(string language)
        {
            Language = language;
        }

        /// <summary>
        /// Add a key to the collection
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The reference to the key.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public KeyLangReference Add(IKey key)
        {
            key.Reference.Language = this;
            Keys.Add(key.Reference.Key, key);

            return key.Reference;
        }

        /// <summary>
        /// Get the key if available else result is null.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The key if it is in the collection else it will return null.</returns>
        public IKey GetKey(string key)
        {
            return Keys.GetValueOrDefault(key);
        }
        /// <summary>
        /// Try to get the key if available
        /// </summary>
        /// <param name="keyString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool TryGetKey(string keyString, out IKey key)
        {
            return Keys.TryGetValue(keyString, out key);
        }

        /// <summary>
        /// Get all the keys that are in the language.
        /// </summary>
        /// <returns>A list of keys.</returns>
        public IList<IKey> GetAllKeys()
        {
            return Keys.Values.ToList();
        }
        /// <summary>
        /// Get all the keys that are in the language of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> GetAllKeysOfType<T>()
            where T : IKey
        {
            return GetAllKeys().OfType<T>().ToList();
        }

        /// <summary>
        /// Get A list of all the sub keys (recursive).
        /// </summary>
        /// <param name="key"></param>
        /// <param name="includeSelf"></param>
        /// <returns></returns>
        public IList<IKey> GetAllSubKeys(string key, bool includeSelf = false)
        {
            var keyObject = GetKey(key);
            List<IKey> keys = !(keyObject is KeyGroup)
                ? new List<IKey>()
                : (GetKey(key) as KeyGroup)?.GetSubKeyRefs()
                .Select(k => GetKey(k.Key))
                .Where(k => k != null)
                .ToList();

            if (includeSelf) keys.Add(GetKey(key));

            List<IKey> result = keys.ToList();

            GetAllSubKeys(keys, result);
            return result;
        }
        private void GetAllSubKeys(IList<IKey> previousKeys, List<IKey> result)
        {
            List<IKey> keys = previousKeys.OfType<KeyGroup>()
                .SelectMany(k => k.GetSubKeyRefs())
                .Select(k => GetKey(k.Key))
                .Where(k => k != null && !result.Contains(k))
                .ToList();

            if (keys.Count() > 0)
            {
                result.AddRange(keys);
                GetAllSubKeys(keys, result);
            }
        }
        /// <summary>
        /// Get A list of all the sub keys of type T (recursive).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="includeSelf"></param>
        /// <returns></returns>
        public IList<T> GetAllSubKeysOfType<T>(string key, bool includeSelf = false)
        {
            return GetAllSubKeys(key, includeSelf).OfType<T>().ToList();
        }

        /// <summary>
        /// Returns true when the key is in this collection.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return Keys.ContainsKey(key);
        }

        public bool IsKeyInGroup(string key, string group)
        {
            IKey keyObject = GetKey(key);

            if (key == null) return false;

            var subKeys = GetAllSubKeys(group, true);
            var result = subKeys.Any((gk) => keyObject.Equals(gk));

            if (result || !(keyObject is IAlias))
            {
                return result;
            }

            // Is any of the keys an alias and is one of them the parent of the key;
            return subKeys.OfType<IAlias>()
                .Any((alias) => alias.IsAlias(keyObject as IAlias));
        }
        public bool IsKeyInGroup(KeyLangReference key, KeyLangReference group)
        {
            return IsKeyInGroup(key.Key, group.Key);
        }

        /// <summary>
        /// Create a reference for this key and language pair.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public KeyLangReference CreateReference(string key)
        {
            return new KeyLangReference { Key = key, Language = this };
        }


        /// <summary>
        /// Add a filter for this language.
        /// </summary>
        /// <param name="filter">A filter that can be used for this language.</param>
        public void AddFilter(IFilter filter)
        {
            filter.SetLanguage(this);
            Filters.Add(filter);
        }
        /// <summary>
        /// Find a filter of type.
        /// </summary>
        /// <typeparam name="TFilter">A type of filter</typeparam>
        /// <returns>The filter if found.</returns>
        public TFilter FindFilter<TFilter>()
            where TFilter : IFilter
        {
            return Filters.OfType<TFilter>().FirstOrDefault();
        }
        /// <summary>
        /// Find all filters of type.
        /// </summary>
        /// <typeparam name="TFilter">A type of filter.</typeparam>
        /// <returns>A array of all the filters.</returns>
        public TFilter[] FindFilters<TFilter>()
            where TFilter : IFilter
        {
            return Filters.OfType<TFilter>().ToArray();
        }
    }
}
