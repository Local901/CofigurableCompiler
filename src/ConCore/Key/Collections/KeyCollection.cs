using System;
using System.Collections.Generic;
using System.Linq;

namespace ConCore.Key.Collections
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
            if (key == null) return null;
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
        /// Get all the keys that are related to the refered key.
        /// </summary>
        /// <param name="key">Key reference.</param>
        /// <param name="includeSeft">Should the list include the referenced key itself?</param>
        /// <returns></returns>
        public IList<IKey> GetAllSubKeys(KeyLangReference key, bool includeSeft = false)
        {
            var keys = GetLanguage(key.Lang)?.GetAllSubKeys(key.Key, includeSeft);
            if (keys == null) return new List<IKey>();
            return keys;
        }
        /// <summary>
        /// Get all the keys that are related to the refered key.
        /// </summary>
        /// <typeparam name="T">Type of key.</typeparam>
        /// <param name="key">Key reference.</param>
        /// <param name="includeSelf">Should the list include the referenced key itself?</param>
        /// <returns></returns>
        public IList<T> GetAllSubKeysOfType<T>(KeyLangReference key, bool includeSelf = false)
            where T : IKey
        {
            return GetAllSubKeys(key, includeSelf).OfType<T>().ToList();
        }
        /// <summary>
        /// Get all the keys that are related to the refered key.
        /// </summary>
        /// <typeparam name="T">Type of key.</typeparam>
        /// <param name="key">Key reference.</param>
        /// <param name="includeSelf">Should the list include the referenced key itself?</param>
        /// <returns></returns>
        public IList<T> GetAllSubKeysOfType<T>(IEnumerable<KeyLangReference> keys, bool includeSelf = false)
            where T : IKey
        {
            return keys.SelectMany(key => GetAllSubKeys(key, includeSelf).OfType<T>().ToList())
                .Distinct().ToList();
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
