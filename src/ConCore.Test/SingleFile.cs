using ConCore.Blocks;
using ConCore.FileInfo;
using ConCore.Key;
using ConCore.Key.Collections;
using ConCore.Key.Components;
using ConCore.Lexing;
using ConCore.Parsing;
using ConCore.Parsing.Simple;
using ConCore.Tools.Contracts;
using Moq;

namespace ConCore.Test
{
    public class SingleFile
    {
        private readonly string CCode = "void main() {int i = 5;return i;}";

        private Mock<ILanguageLoader> languageLoaderMock;

        [SetUp]
        public void Setup()
        {
            languageLoaderMock = new Mock<ILanguageLoader>();
            languageLoaderMock.Setup((loader) => loader.LoadConfig(It.IsAny<FileData>(), It.IsAny<KeyCollection>()))
                .Returns<FileData, KeyCollection>((file, collection) =>
                {
                    Language language = collection.GetLanguage("c_lang");
                    if (language != null)
                    {
                        return language;
                    }

                    language = new Language("c_lang");
                    var tOpenBrace = language.AddKey(new Token("open_brace", "{"));
                    var tCloseBrace = language.AddKey(new Token("close_brace", "}"));
                    var tOpenBracket = language.AddKey(new Token("open_bracket", "\\("));
                    var tCloseBracket = language.AddKey(new Token("close_bracket", "\\)"));
                    var tIntLiteral = language.AddKey(new Token("int_literal", "[0-9]"));
                    var tIdentifier = language.AddKey(new Token("identifier", "[a-zA-Z_][a-zA-Z0-9_]*"));
                    var tSemiCollon = language.AddKey(new Token("semi_collon", ";"));
                    var tAssignOp = language.AddKey(new Token("assignment_opperator", "="));

                    var cVarDef = language.AddKey(new Construct("variable_definition", new OrderComponent(new List<Component>
                    {
                        new ValueComponent(tIdentifier, "type"),
                        new ValueComponent(tIdentifier, "name"),
                    })));

                    var gVariable = new KeyGroup("variables", new List<KeyLangReference>
                    {
                        tIdentifier,
                        cVarDef,
                    });
                    language.AddKey(gVariable);

                    var gValue = new KeyGroup("values", new List<KeyLangReference>
                    {
                        tIdentifier,
                        tIntLiteral,
                    });
                    language.AddKey(gValue);

                    var cAssignment = language.AddKey(new Construct("assignment", new OrderComponent(new List<Component>
                    {
                        new ValueComponent(gVariable.Reference),
                        new ValueComponent(tAssignOp),
                        new ValueComponent(gValue.Reference),
                    })));

                    var cReturn = language.AddKey(new Construct("return", new OrderComponent(new List<Component>
                    {
                        new ValueComponent(tIdentifier),
                        new ValueComponent(gValue.Reference),
                    })));

                    var cLine = language.AddKey(new Construct("line", new OrderComponent(new List<Component>
                    {
                        new AnyComponent(new List<Component>
                        {
                            new ValueComponent(cAssignment),
                            new ValueComponent(cReturn),
                        }),
                        new ValueComponent(tSemiCollon),
                    })));

                    var cFuncDef = language.AddKey(new Construct("function_definition", new OrderComponent(new List<Component>
                    {
                        new ValueComponent(tIdentifier, "return_type"),
                        new ValueComponent(tIdentifier, "name"),
                        new ValueComponent(tOpenBracket),
                        new ValueComponent(tCloseBracket),
                        new ValueComponent(tOpenBrace),
                        new RepeatComponent(new ValueComponent(cLine)),
                        new ValueComponent(tCloseBrace),
                    })));


                    language.StartingKeyReference = cFuncDef;

                    collection.AddLanguage(language);
                    return language;
                });
        }

        [Test]
        public void LexFile()
        {
            KeyCollection collection = new KeyCollection();
            var language = languageLoaderMock.Object.LoadConfig(null, collection);
            var refs = language.AllKeys<Token>().Select(k => k.Reference);

            ILexer lexer = new SimpleLexer(new StreamChunkReader(new StreamReader(CCode)), language);

            var result = new List<IBlock>();
            IList<LexResult> blocks;
            while((blocks = lexer.TryNextBlock(refs)) != null && blocks.Count > 0)
            {
                Assert.That(blocks, Has.Count.EqualTo(1));
                result.Add(blocks[0].Block);
            }
            Assert.That(blocks, Is.Not.Null);

            Assert.That(result, Has.Count.EqualTo(14));
            Assert.That(result[0].Key.Reference.Key, Is.EqualTo("identifier"));
            Assert.That(result[1].Key.Reference.Key, Is.EqualTo("identifier"));
            Assert.That(result[2].Key.Reference.Key, Is.EqualTo("open_bracket"));
            Assert.That(result[3].Key.Reference.Key, Is.EqualTo("close_bracket"));
            Assert.That(result[4].Key.Reference.Key, Is.EqualTo("open_brace"));
            Assert.That(result[5].Key.Reference.Key, Is.EqualTo("identifier"));
            Assert.That(result[6].Key.Reference.Key, Is.EqualTo("identifier"));
            Assert.That(result[7].Key.Reference.Key, Is.EqualTo("assignment_opperator"));
            Assert.That(result[8].Key.Reference.Key, Is.EqualTo("int_literal"));
            Assert.That(result[9].Key.Reference.Key, Is.EqualTo("semi_collon"));
            Assert.That(result[10].Key.Reference.Key, Is.EqualTo("identifier"));
            Assert.That(result[11].Key.Reference.Key, Is.EqualTo("identifier"));
            Assert.That(result[12].Key.Reference.Key, Is.EqualTo("semi_collon"));
            Assert.That(result[13].Key.Reference.Key, Is.EqualTo("close_brace"));
        }

        [Test]
        public void ParseFile()
        {
            KeyCollection collection = new KeyCollection();
            var language = languageLoaderMock.Object.LoadConfig(null, collection);

            ILexer lexer = new SimpleLexer(new StreamChunkReader(new StreamReader(CCode)), language);
            IParser parser = new SimpleParser(lexer, new ParseArgFactory(collection), collection);

            var languageStart = language.StartingKeyReference;

            Assert.That(language, Is.Not.Null);

            IBlock? result = parser.DoParse(languageStart);

            Assert.That(result, Is.Not.Null);
        }
    }
}