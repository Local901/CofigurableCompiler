using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Key.Collections
{
    public interface ILanguage : IAliasManager
    {
        string Name { get; }
        KeyLangReference StartingKeyReference { get; }

        /// <summary>
        /// Add a key to the collection
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The reference to the key.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        KeyLangReference AddKey(IKey key);
        /// <summary>
        /// Get the key if available else result is null.
        /// </summary>
        /// <param name="keyReference">Reference to a key.</param>
        /// <returns>The key if it is in the collection else it will return null.</returns>
        IKey? GetKey(KeyLangReference keyReference);
        /// <summary>
        /// Returns true when the key is in this collection.
        /// </summary>
        /// <param name="key">Reference to a key.</param>
        /// <returns>True if the key is contained in the language.</returns>
        bool ContainsKey(KeyLangReference keyReference);
        /// <summary>
        /// Get all the keys that are in the language.
        /// </summary>
        /// <returns>A list of all keys.</returns>
        IEnumerable<IKey> AllKeys();
        /// <summary>
        /// Get all the keys that are in the language of type T.
        /// </summary>
        /// <typeparam name="T">The <see cref="IKey"/> type to return just that type of key.</typeparam>
        /// <returns>A list of all keys of type T.</returns>
        IEnumerable<T> AllKeys<T>() where T : IKey;
        /// <summary>
        /// Get A list of all the sub keys (recursive, no KeyGroups).
        /// </summary>
        /// <param name="key">The reference to the key.</param>
        /// <param name="includeSelf">Chould the resultinh list include the key.</param>
        /// <returns>List of keys that are the children of the refered key.</returns>
        IEnumerable<IKey> AllChildKeys(KeyLangReference? keyReference, bool includeSelf = false);
        /// <summary>
        /// Get A list of all the sub keys of type T (recursive, no KeyGroups).
        /// </summary>
        /// <typeparam name="T">A <see cref="IKey"/> type to return just those types.</typeparam>
        /// <param name="key">The reference to the key.</param>
        /// <param name="includeSelf">Chould the resultinh list include the key.</param>
        /// <returns>List of keys of type T that are the children of the refered key.</returns>
        IEnumerable<T> AllChildKeys<T>(KeyLangReference? keyReference, bool includeSelf = false)
            where T : IKey;
        /// <summary>
        /// Check if a key is part of a group. The group can be any key.
        /// </summary>
        /// <param name="keyReference">The reference to the key.</param>
        /// <param name="groupReference">The reference to the group.</param>
        /// <returns>True if the key is a child of the group. (recursive)</returns>
        bool IsKeyInGroup(
            KeyLangReference keyReference,
            KeyLangReference groupReference
        );
        /// <summary>
        /// Create a reference for this key and language pair.
        /// </summary>
        /// <param name="key">The key identifier for the key reference.</param>
        /// <returns>A key language reference to this language.</returns>
        KeyLangReference CreateReference(string keyIdentifier);
    }
}
