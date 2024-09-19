using ConCli.Common;
using ConCore.Blocks;
using ConCore.Key.Collections;
using ConCore.Key.Modifiers;
using ConCore.Lexing;
using ConCore.Parsing;
using ConCore.Parsing.Simple;
using ConLine.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCli.Steps
{
    public delegate IBlock ParseFile(PathInstance path, KeyCollection keyCollection, LangCollection langCollection);

    public class FileParserStep: FunctionCallStep<ParseFile>
    {
        public FileParserStep(string name)
            : base(name, new string[] { "keyCollection" })
        {
            FunctionHandler = ParseFile;
        }

        private IBlock ParseFile(PathInstance path, KeyCollection keyCollection, LangCollection langCollection)
        {
            var fileReader = new StreamChunkReader(new StreamReader(File.OpenRead((string)path)));
            var lexer = new SimpleLexer(fileReader, keyCollection);
            var parser = new SimpleParser(lexer, new ParseArgFactory(keyCollection), keyCollection);

            var startFilter = langCollection.FindFilter<LanguageStart>();
            if (startFilter == null)
            {
                throw new Exception($"Language {langCollection.Language} doesn't contain a start construct");
            }

            var result = parser.DoParse(startFilter.GetKeyReference());
            if (result == null)
            {
                throw new Exception($"Failed to parse {path}");
            }
            return result;
        }
    }
}
