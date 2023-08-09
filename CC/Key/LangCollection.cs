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
            key.Reference.Lang = Language;
            Keys.Add(key.Reference.Key, key);

            key.GetSubKeys().ForEach(subKey =>
            {
                if (ContainsKey(subKey.Reference.Key)) return;
                try
                {
                    Add(subKey);
                }
                catch (Exception) { }
            });

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
        /// Get all prominent keys in the language.
        /// </summary>
        /// <returns></returns>
        public IList<IKey> GetAllProminentKeys()
        {
            return Keys.Values.Select(k => k.ProminentKey)
                .Distinct().ToList();
        }
        /// <summary>
        /// Get all prominent keys in the language of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> GetAllProminentKeys<T>()
            where T : IKey
        {
            return GetAllProminentKeys().OfType<T>().ToList();
        }

        /// <summary>
        /// Get the prominent key of the key if possible. If no key was found result is null.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IKey GetProminentKey(string key)
        {
            return GetKey(key)?.ProminentKey;
        }
        /// <summary>
        /// Try to get the prominent key.
        /// </summary>
        /// <param name="keyString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool TryGetProminentKey(string keyString, out IKey key)
        {
            bool result = TryGetKey(keyString, out key);
            key = key?.ProminentKey;
            return result;
        }

        /// <summary>
        /// Get A list of all the sub keys of type T (recursive).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="includeSelf"></param>
        /// <returns></returns>
        public IList<T> GetAllSubKeys<T>(string key, bool includeSelf = false)
        {
            return GetAllSubKeys(key, includeSelf).OfType<T>().ToList();
        }
        /// <summary>
        /// Get A list of all the sub keys (recursive).
        /// </summary>
        /// <param name="key"></param>
        /// <param name="includeSelf"></param>
        /// <returns></returns>
        public IList<IKey> GetAllSubKeys(string key, bool includeSelf = false)
        {
            List<IKey> keys = GetKey(key).GetSubKeyRefs()
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
            List<IKey> keys = previousKeys.SelectMany(k => k.GetSubKeyRefs())
                .Select(k => GetKey(k.Key))
                .Where(k => k != null && !result.Contains(k))
                .ToList();

            if (keys.Count() > 0)
            {
                result.AddRange(keys);
                GetAllSubKeys(keys, result);
            }
        }

        public IList<T> GetAllProminentSubKeys<T>(string key, bool includeSelf = false) where T : IKey
        {
            return GetAllProminentSubKeys(key, includeSelf).OfType<T>().ToList();
        }
        public IList<IKey> GetAllProminentSubKeys(string key, bool includeSelf = false)
        {
            List<IKey> keys = GetKey(key)?.GetSubKeyRefs()
                .Select(k => GetKey(k.Key))
                .Where(k => k != null)
                .ToList();

            if (keys == null) return new List<IKey>();
            if (includeSelf) keys.Add(GetKey(key).ProminentKey);

            List<IKey> result = keys.Select(k => k.ProminentKey)
                .Distinct()
                .ToList();

            GetAllProminentSubKeys(keys, result);
            return result;
        }
        private void GetAllProminentSubKeys(IList<IKey> previousKeys, List<IKey> result)
        {
            List<IKey> keys = previousKeys.SelectMany(k => k.GetSubKeyRefs())
                .Select(k => GetKey(k.Key)?.ProminentKey)
                .Where(k => k != null && !result.Contains(k))
                .ToList();

            if (keys.Count() > 0)
            {
                result.AddRange(keys);
                GetAllProminentSubKeys(keys, result);
            }
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

            return GetAllSubKeys(group, true).Any(gk => keyObject.Equals(gk));
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
            return new KeyLangReference { Key = key, Lang = Language };
        }
    }
}
