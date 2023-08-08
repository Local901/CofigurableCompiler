using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Key
{
    public class KeyCollection
    {
        private Dictionary<string, LangCollection> Languages = new Dictionary<string, LangCollection>();

        public KeyCollection() { }

        /// <summary>
        /// Add a new language to the collection.
        /// </summary>
        /// <param name="language"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddLanguage(LangCollection language)
        {
            Languages.Add(language.Language, language);
        }

        /// <summary>
        /// Get the language collection of the language. If not found it return null.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public LangCollection GetLanguage(string language)
        {
            return Languages.GetValueOrDefault(language);
        }

        /// <summary>
        /// Get the key that corespondes to the provided reference. If not found return null.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IKey GetKey(KeyLangReference key)
        {
            return GetLanguage(key.Lang)?.GetKey(key.Key);
        }

        /// <summary>
        /// Get all keys in all the languages.
        /// </summary>
        /// <returns>List of distinct keys</returns>
        public List<IKey> GetAllKeys()
        {
            return Languages.Values
                .SelectMany(lang => lang.GetAllKeys())
                .ToList();
        }
        /// <summary>
        /// Get all keys of type T.
        /// </summary>
        /// <typeparam name="T">Type that inherited IKey.</typeparam>
        /// <returns>List of type T.</returns>
        public List<T> GetAllKeysOfType<T>()
            where T : IKey
        {
            return GetAllKeys().OfType<T>()
                .ToList();
        }
        /// <summary>
        /// Get all prominent keys of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetAllProminentKeysOfType<T>()
            where T : IKey
        {
            return GetAllKeys().Select(k => k.ProminentKey)
                .Distinct()
                .OfType<T>()
                .ToList();
        }

        public IList<IKey> GetAllSubKeys(KeyLangReference key, bool includeSeft = false)
        {
            var keys = GetLanguage(key.Lang)?.GetAllSubKeys(key.Key, includeSeft);
            if (keys == null) return new List<IKey>();
            return keys;
        }

        /// <summary>
        /// Get all keys related to the key.
        /// </summary>
        /// <param name="key">A key reference </param>
        /// <param name="includeSelf">Should the resulting list include the provided key?</param>
        /// <returns>List of related prominent keys.</returns>
        public List<IKey> GetAllProminentSubKeys(KeyLangReference key, bool includeSelf = false)
        {
            var keys = GetLanguage(key.Lang)?.GetAllProminentSubKeys(key.Key, includeSelf)?.ToList();
            if (keys == null) keys = new List<IKey>();
            return keys;
        }
        /// <summary>
        /// Get all keys related to the keys.
        /// </summary>
        /// <param name="keys">A list of keys</param>
        /// <param name="includeSelf">Should the resulting list include the provided key?</param>
        /// <returns>List of distinct related prominent keys.</returns>
        public List<IKey> GetAllProminentSubKeys(IEnumerable<KeyLangReference> keys, bool includeSelf = false)
        {
            return keys.SelectMany(key => GetAllProminentSubKeys(key, includeSelf))
                .ToList();
        }
        /// <summary>
        /// Get all objects of type T related to the key.
        /// </summary>
        /// <typeparam name="T">Type that inherited IKey.</typeparam>
        /// <param name="key">A key reference.</param>
        /// <param name="includeSelf">Should the resulting list include the provided key?</param>
        /// <returns>List of type T related to the key.</returns>
        public List<T> GetAllProminentSubKeysOfType<T>(KeyLangReference key, bool includeSelf = false)
            where T : IKey
        {
            return GetAllProminentSubKeys(key, includeSelf).OfType<T>()
                .ToList();
        }
        /// <summary>
        /// Get all objects of type T related to the keys.
        /// </summary>
        /// <typeparam name="T">Type that inherited IKey.</typeparam>
        /// <param name="keys">A list of keys.</param>
        /// <param name="includeSelf">Should the resulting list include the provided key?</param>
        /// <returns>List of type T related to the keys.</returns>
        public List<T> GetAllProminentSubKeysOfType<T>(IEnumerable<KeyLangReference> keys, bool includeSelf = false)
            where T : IKey
        {
            return GetAllProminentSubKeys(keys, includeSelf).OfType<T>()
                .ToList();
        }

        /// <summary>
        /// Check if key is/ is part of a certain groupKey.
        /// </summary>
        /// <param name="key">The key reference.</param>
        /// <param name="group">The group reference.</param>
        /// <returns>True is key is related to the groep.</returns>
        public bool IsKeyInGroup(KeyLangReference key, KeyLangReference group)
        {
            return GetAllSubKeys(group, true)
                .Select(k => k.Reference)
                .Contains(key);
        }
        /// <summary>
        /// Check if key is/ is part of a certain groupKey.
        /// </summary>
        /// <param name="key">The key Object.</param>
        /// <param name="group">The group reference.</param>
        /// <returns>True is key is related to the groep.</returns>
        public bool IsKeyInGroup(IKey key, KeyLangReference group)
        {
            return IsKeyInGroup(key.Reference, group);
        }
    }
}
