using CC.FileInfo;
using CC.Key;
using CC.Tools;
using CC.Tools.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Test.Mock
{
    internal class MultyFileParserPartialMock : MultyFileParser
    {
        private Func<FileData, string> FileReader;

        public MultyFileParserPartialMock(ILanguageLoader languageLoader, Func<FileData, string> fileReader)
            : base(languageLoader)
        {
            FileReader = fileReader;
        }

        protected override ILexer CreateLexer(FileData file, KeyCollection keyCollection)
        {
            return new FileLexer(FileReader(file), keyCollection);
        }
    }
}
