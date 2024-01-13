using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Key
{
    public interface IAliased<TKey, TValue>
        where TKey: IKey, IAliased<TKey, TValue>
    {
        /// <summary>
        /// List of aliasses.
        /// </summary>
        IReadOnlyList<IAliased<TKey, TValue>> Aliasses { get; }

        /// <summary>
        /// Check the value if it corresponds with this key.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value corresponds with the key else false.</returns>
        bool Check(TValue value);

        /// <summary>
        /// Add an allias to this key.
        /// </summary>
        /// <param name="alias"></param>
        void AddAlias(IAliased<TKey, TValue> alias);

        /// <summary>
        /// Find all aliasses that correspond with the value.
        /// </summary>
        /// <param name="value">The value to check with.</param>
        /// <param name="includeSelf">Chould the search include this object.</param>
        /// <returns>An array of keys.</returns>
        IKey[] FindAliasses(TValue value, bool includeSelf = true);
    }
}
