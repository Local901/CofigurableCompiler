using CC.Key;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC
{
    public interface ILanguageLoader
    {
        /// <summary>
        /// Load the language into the provided key collection
        /// </summary>
        /// <param name="keyCollection">The key collection to be populated with the language.</param>
        public void LoadLanguage(KeyCollection keyCollection);
    }
}
