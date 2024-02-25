using ConCore.FileInfo;
using ConCore.Key.Collections;
using ConCore.Lexing;
using ConCore.Tools;
using ConCore.Tools.Contracts;

namespace ConCore.Test.Mock
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
            return new SimpleLexer(FileReader(file), keyCollection);
        }
    }
}
