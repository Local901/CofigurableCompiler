using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Key.Collections
{
    public interface IAliasManager
    {
        /// <summary>
        /// Set an alias relation between the two references.
        /// </summary>
        /// <param name="keyReference">Parent key reference.</param>
        /// <param name="aliasReference">Reference to the alias of the key.</param>
        void SetAlias(KeyLangReference keyReference, KeyLangReference aliasReference);
        /// <summary>
        /// Get the direct aliases of a key.
        /// </summary>
        /// <param name="keyReference">Key reference to find the aliases for.</param>
        /// <returns>List of key references that are an alias to the key reference.</returns>
        IEnumerable<KeyLangReference> GetAliases(KeyLangReference keyReference);
        /// <summary>
        /// Check if one key is the alias of another. This is a recursive search.
        /// </summary>
        /// <param name="keyReference">Suspected parent reference.</param>
        /// <param name="suspectedAliasReference">Suspected alias reference.</param>
        /// <returns></returns>
        bool IsAliasOf(KeyLangReference keyReference, KeyLangReference aliasReference);
    }
}
