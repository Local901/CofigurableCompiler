using CC.Blocks;
using CC.FileInfo;
using CC.Key;
using CC.Key.ComponentTypes;
using CC.Key.Modifiers;
using CC.Test.Mock;
using CC.Tools;
using CC.Tools.Contracts;
using Moq;

namespace CC.Test
{
    public class Tests
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
                    LangCollection language = collection.GetLanguage("c_lang");
                    if (language != null)
                    {
                        return language;
                    }

                    language = new LangCollection("c_lang");
                    var tOpenBrace = language.Add(new Token("open_brace", "{"));
                    var tCloseBrace = language.Add(new Token("close_brace", "}"));
                    var tOpenBracket = language.Add(new Token("open_bracket", "\\("));
                    var tCloseBracket = language.Add(new Token("close_bracket", "\\)"));
                    var tIntLiteral = language.Add(new Token("int_literal", "[0-9]"));
                    var tIdentifier = language.Add(new Token("identifier", "[a-zA-Z_][a-zA-Z0-9_]*"));
                    var tSemiCollon = language.Add(new Token("semi_collon", ";"));
                    var tAssignOp = language.Add(new Token("assignment_opperator", "="));

                    var cVarDef = language.Add(new Construct("variable_definition", new OrderComponent(new List<IComponent>
                    {
                        new ValueComponent(tIdentifier, "type"),
                        new ValueComponent(tIdentifier, "name"),
                    })));

                    var gVariable = new KeyGroup("varibles", new List<KeyLangReference>
                    {
                        tIdentifier,
                        cVarDef,
                    });
                    language.Add(gVariable);

                    var gValue = new KeyGroup("values", new List<KeyLangReference>
                    {
                        tIdentifier,
                        tIntLiteral,
                    });
                    language.Add(gValue);

                    var cAssignment = language.Add(new Construct("assignment", new OrderComponent(new List<IComponent>
                    {
                        new ValueComponent(gVariable.Reference),
                        new ValueComponent(tAssignOp),
                        new ValueComponent(gValue.Reference),
                    })));

                    var cReturn = language.Add(new Construct("return", new OrderComponent(new List<IComponent>
                    {
                        new ValueComponent(tIdentifier),
                        new ValueComponent(gValue.Reference),
                    })));

                    var cLine = language.Add(new Construct("line", new OrderComponent(new List<IComponent>
                    {
                        new AnyComponent(new List<IComponent>
                        {
                            new ValueComponent(cAssignment),
                            new ValueComponent(cReturn),
                        }),
                        new ValueComponent(tSemiCollon),
                    })));

                    var cFuncDef = language.Add(new Construct("function_definition", new OrderComponent(new List<IComponent>
                    {
                        new ValueComponent(tIdentifier, "return_type"),
                        new ValueComponent(tIdentifier, "name"),
                        new ValueComponent(tOpenBracket),
                        new ValueComponent(tCloseBracket),
                        new ValueComponent(tOpenBrace),
                        new RepeatComponent(new ValueComponent(cLine)),
                        new ValueComponent(tCloseBrace),
                    })));


                    language.AddFilter(new LanguageStart(new LanguageStartArgs
                    {
                        KeyReference = cFuncDef,
                    }));

                    collection.AddLanguage(language);
                    return language;
                });
        }

        [Test]
        public void LexFile()
        {
            KeyCollection collection = new KeyCollection();
            var language = languageLoaderMock.Object.LoadConfig(null, collection);
            var refs = language.GetAllKeysOfType<Token>().Select(k => k.Reference);

            ILexer lexer = new FileLexer(CCode, collection);

            var result = new List<IBlock>();
            IBlock block;
            while(lexer.TryNextBlock(out block, refs))
            {
                result.Add(block);
            }

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
        public void ParseSingleFile()
        {
            IMultiFileParser parser = new MultyFileParserPartialMock(languageLoaderMock.Object, (file) => CCode);
            var result = parser.Parse("test");
            Assert.Pass();
        }
    }
}