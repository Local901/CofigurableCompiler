using ConCore.Key.Collections;
using System;

namespace ConCore.Key.Modifiers
{
    /// <summary>
    /// Objects to allow for custom interactions with the langauge.
    /// </summary>
    public abstract class IFilter : IKey
    {
        protected LangCollection Language { get; private set; }

        internal void SetLanguage(LangCollection language)
        {
            if (Language != null)
            {
                throw new Exception("Can't change the language of a filter when it has already been assigned.");
            }
            Language = language;
        }
    }
}
