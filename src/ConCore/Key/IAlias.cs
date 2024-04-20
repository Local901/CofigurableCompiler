using System.Collections.Generic;

namespace ConCore.Key
{
    public interface IAlias
    {
        /// <summary>
        /// Is the allias of this type?
        /// </summary>
        /// <param name="allias"></param>
        /// <returns>True when the allias is this or one of the child aliasses else false.</returns>
        bool IsAlias(IAlias allias);

        /// <summary>
        /// Get all root allias objects.
        /// </summary>
        /// <returns></returns>
        IList<IAlias> RootAliases();

        /// <summary>
        /// Find all aliasses that correspond with the value.
        /// </summary>
        /// <param name="value">The value to check with.</param>
        /// <param name="includeSelf">Chould the search include this object.</param>
        /// <returns>An array of keys. Returns an empty array when value not of correct type.</returns>
        IKey[] FindAliases(object value, bool includeSelf = true);
    }

    public interface IAlias<TKey, TValue> : IAlias
        where TKey: IKey, IAlias<TKey, TValue>
    {
        /// <summary>
        /// List of aliasses.
        /// </summary>
        IReadOnlyList<IAlias<TKey, TValue>> Aliasses { get; }

        /// <summary>
        /// List of parent aliasses.
        /// </summary>
        IReadOnlyList<IAlias<TKey, TValue>> AliasParents { get; }

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
        void AddAlias(IAlias<TKey, TValue> alias);

        /// <summary>
        /// Add a parent alias.
        /// </summary>
        /// <param name="alias"></param>
        void AddParentAlias(IAlias<TKey, TValue> alias);

        /// <summary>
        /// Find all aliasses that correspond with the value.
        /// </summary>
        /// <param name="value">The value to check with.</param>
        /// <param name="includeSelf">Chould the search include this object.</param>
        /// <returns>An array of keys.</returns>
        IKey[] FindAliasses(TValue value, bool includeSelf = true);

        /// <summary>
        /// Is the allias of this type?
        /// </summary>
        /// <param name="allias"></param>
        /// <returns>True when the allias is this or one of the child aliasses else false.</returns>
        bool IsAlias(IAlias<TKey, TValue> allias);

        /// <summary>
        /// Get all root allias objects.
        /// </summary>
        /// <returns></returns>
        IList<IAlias<TKey, TValue>> RootAlliasses();
    }
}
