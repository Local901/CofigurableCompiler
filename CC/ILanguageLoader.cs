using CC.FileInfo;
using CC.Key;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC
{
    public interface ILanguageLoader
    {
        /// <summary>
        /// Load a language configuration into the key collection. And add an language starting point to the file.
        /// </summary>
        /// <param name="file">All information about a file.</param>
        /// <param name="keyCollection">The collection.</param>
        public void LoadConfig(FileData file, KeyCollection keyCollection);
    }
}
