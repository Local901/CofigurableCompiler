using BranchList;
using CC.Blocks;
using CC.Key;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CC
{
    public class MultyFileParser : IMultiFileParser
    {
        private ILanguageLoader languageLoader;

        public MultyFileParser(ILanguageLoader languageLoader)
        {
            this.languageLoader = languageLoader;
        }

        public IBlock[] Parse(string[] filePaths, string[] configPaths)
        {
            var keyCollection = new KeyCollection();
            configPaths.ForEach(path => languageLoader.LoadConfig(path, keyCollection));

            var blockList = new List<IBlock>();


            throw new NotImplementedException();
        }

        protected virtual ILexer CreateLexer(string fileContent, KeyCollection keyCollection)
        {
            return new FileLexer(fileContent, keyCollection);
        }

        protected virtual IParser CreateParser(ILexer lexer, KeyCollection keyCollection)
        {
            return new FileParser(lexer, keyCollection);
        }
    }
}
