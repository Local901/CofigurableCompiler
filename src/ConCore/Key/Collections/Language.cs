using System;
using System.Collections.Generic;
using System.Linq;

namespace ConCore.Key.Collections
{
    public class Language : ILanguage
    {
        public string Name { get; }
        public KeyLangReference StartingKeyReference { get; set; }

        private readonly Dictionary<string, IKey> Keys = new Dictionary<string, IKey>();
        private readonly Dictionary<KeyLangReference, List<KeyLangReference>> Aliases = new Dictionary<KeyLangReference, List<KeyLangReference>>();

        public Language(string language)
        {
            Name = language;
        }

        public KeyLangReference AddKey(IKey key)
        {
            key.Reference = CreateReference(key.Name);
            Keys.Add(key.Reference.Key, key);

            return key.Reference;
        }

        public IKey? GetKey(KeyLangReference keyReference)
        {
            if (keyReference.Lang != Name) return null;
            return Keys.GetValueOrDefault(keyReference.Key);
        }

        public bool ContainsKey(KeyLangReference keyReference)
        {
            if (keyReference.Lang != Name) return false;
            return Keys.ContainsKey(keyReference.Key);
        }

        public IEnumerable<IKey> AllKeys()
        {
            return Keys.Values;
        }

        public IEnumerable<T> AllKeys<T>()
            where T : IKey
        {
            return AllKeys().OfType<T>();
        }

        public IEnumerable<IKey> AllChildKeys(KeyLangReference? keyReference, bool includeSelf = false)
        {
            if (keyReference == null) return new List<IKey>();

            var keyObject = GetKey(keyReference);
            List<IKey> keys = keyObject is not KeyGroup
                ? new List<IKey>()
                : ((KeyGroup)keyObject).GetSubKeyRefs()
                    .Select(k => GetKey(k))
                    .OfType<IKey>()
                    .ToList();

            if (includeSelf && keyObject != null) keys.Add(keyObject);

            List<IKey> result = keys.ToList();

            GetAllSubKeys(keys, result);
            return result;
        }
        private void GetAllSubKeys(IList<IKey> previousKeys, List<IKey> result)
        {
            List<IKey> keys = previousKeys.OfType<KeyGroup>()
                .SelectMany(k => k.GetSubKeyRefs())
                .Select(k => GetKey(k))
                .OfType<IKey>()
                .Where(k => !result.Contains(k))
                .ToList();

            if (keys.Count() > 0)
            {
                result.AddRange(keys);
                GetAllSubKeys(keys, result);
            }
        }

        public IEnumerable<T> AllChildKeys<T>(KeyLangReference? keyReference, bool includeSelf = false)
            where T : IKey
        {
            return AllChildKeys(keyReference, includeSelf).OfType<T>();
        }

        public bool IsKeyInGroup(
            KeyLangReference keyReference,
            KeyLangReference groupReference
        ) {
            if (keyReference == null || groupReference == null) return false;

            IKey? keyObject = GetKey(keyReference);

            if (keyObject == null) return false;

            var subKeys = AllChildKeys(groupReference, true);
            var result = subKeys.Any((gk) => keyObject.Equals(gk));

            if (result)
            {
                return true;
            }

            // Is any of the keys an alias and is one of them the parent of the key;
            return subKeys.Any((alias) => IsAliasOf(alias.Reference, keyObject.Reference));
        }

        public KeyLangReference CreateReference(string key)
        {
            return new KeyLangReference(this, key);
        }

        public void SetAlias(KeyLangReference keyReference, KeyLangReference aliasReference)
        {
            if (Aliases.ContainsKey(keyReference))
            {
                Aliases[keyReference].Add(aliasReference);
                return;
            }
            Aliases.Add(keyReference, new List<KeyLangReference> { aliasReference });
        }

        public IEnumerable<KeyLangReference> GetAliases(KeyLangReference keyReference)
        {
            if (!Aliases.ContainsKey(keyReference))
            {
                return Enumerable.Empty<KeyLangReference>();
            }
            return Aliases[keyReference];
        }

        public bool IsAliasOf(KeyLangReference keyReference, KeyLangReference aliasReference)
        {
            if (keyReference == aliasReference) return true;
            return GetAliases(keyReference).Any((reference) => IsAliasOf(reference, aliasReference));
        }
    }
}
