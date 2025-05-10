using ConCli.Common;
using ConCore.Blocks;
using ConCore.Key.Collections;
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
    public delegate IBlock ParseFile(PathInstance path, KeyCollection keyCollection, Language langCollection);

    public class FileParserStep: FunctionCallStep<ParseFile>
    {
        public FileParserStep(string name)
            : base(name, new string[] { "keyCollection" })
        {
            FunctionHandler = ParseFile;
        }

        private IBlock ParseFile(PathInstance path, KeyCollection keyCollection, Language langCollection)
        {
            var fileReader = new StreamChunkReader(new StreamReader(File.OpenRead((string)path)));
            var lexer = new SimpleLexer(fileReader, langCollection);
            //var parser = new SimpleParser(lexer, new ParseArgFactory(keyCollection), keyCollection);
            var parser = new SimpleParser3(langCollection, lexer);

            var startingKey = langCollection.StartingKeyReference;
            if (startingKey == null)
            {
                throw new Exception($"Language {langCollection.Name} doesn't contain a start construct");
            }

            var result = parser.DoParse(startingKey);
            if (result == null)
            {
                throw new Exception($"Failed to parse {path}");
            }
            return result;
        }
    }
}
