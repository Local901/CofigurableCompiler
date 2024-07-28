namespace ConCore.Key.Modifiers
{
    public abstract class ILanguageFilter : IFilter
    {
        /// <summary>
        /// Find a specific key in the collection.
        /// </summary>
        /// <param name="language">The language to search in.</param>
        /// <returns>A key.</returns>
        public abstract IKey FindKey();

        /// <summary>
        /// Get the reference to the starting key.
        /// </summary>
        /// <returns>A reference to the starting key.</returns>
        public abstract KeyLangReference GetKeyReference();
    }
}
