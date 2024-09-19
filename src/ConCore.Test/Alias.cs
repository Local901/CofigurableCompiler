using ConCore.Blocks.Helpers;
using ConCore.FileInfo;
using ConCore.Key;
using ConCore.Key.Collections;
using ConCore.Key.Components;
using ConCore.Key.Modifiers;
using ConCore.Lexing;
using ConCore.Tools.Contracts;
using Moq;

namespace ConCore.Test
{
    public class Alias
    {
        private Mock<ILanguageLoader> languageLoaderMock;

        [SetUp]
        public void Setup()
        {
            languageLoaderMock = new Mock<ILanguageLoader>();
            languageLoaderMock.Setup((loader) => loader.LoadConfig(It.IsAny<FileData>(), It.IsAny<KeyCollection>()))
                .Returns<FileData, KeyCollection>((file, collection) =>
                {
                    LangCollection? language = collection.GetLanguage("c_lang");
                    if (language != null)
                    {
                        return language;
                    }

                    language = new LangCollection("test_lang");
                    collection.AddLanguage(language);

                    var tLoad = language.Add(new Token("load", "load"));
                    var tPrint = language.Add(new Token("print_token", "print"));

                    var tFunction = language.Add(new Token("function_token", "[\\S]+"));
                    var functionToken = (Token)language.GetKey(tFunction.Key);
                    functionToken.AddAlias(language.GetKey(tLoad.Key) as Token);
                    functionToken.AddAlias(language.GetKey(tPrint.Key) as Token);

                    var tPath = language.Add(new Token("path", "[a-zA-Z]+"));
                    var tText = language.Add(new Token("text", "[\\S].*$"));

                    var cLoadFile = language.Add(new Construct("load_file", new OrderComponent(new List<Component>
                    {
                        new ValueComponent(tLoad),
                        new ValueComponent(tPath, "path"),
                    })));

                    var cPrintLine = language.Add(new Construct("print_text", new OrderComponent(new List<Component>
                    {
                        new ValueComponent(tPrint),
                        new ValueComponent(tPath),
                    })));

                    var languageStart = language.Add(new Construct("structure", new OrderComponent(new List<Component>
                    {
                        new RepeatComponent(new AnyComponent(new List<Component> {
                            new ValueComponent(cLoadFile),
                            new ValueComponent(cPrintLine),
                        })),
                    })));

                    language.AddFilter(new LanguageStart(new LanguageStartArgs
                    {
                        KeyReference = languageStart,
                    }));

                    language.AddFilter(new FileReference(new BlockReader(), new FileReferenceArgs
                    {
                        KeyReference = cLoadFile,
                        ReferenceType = FileReferenceType.Relative,
                        ValuePath = new string[] { "path" },
                    }));

                    return language;
                });
        }

        [Test]
        public void ShouldLexMultipleBlocksAtOnceWhenThereAreAliases()
        {
            var collection = new KeyCollection();
            languageLoaderMock.Object.LoadConfig(null, collection);

            var lexer = new SimpleLexer(
                new StreamChunkReader(new StreamReader($"print hello world\nload file")),
                collection
                );
            var language = collection.GetLanguage("test_lang");

            var blocks = lexer.TryNextBlock(language.CreateReference("function_token"));

            Assert.That(blocks, Has.Count.EqualTo(2)); // function_token block and print_token block
            Assert.That(blocks.Select((b) => b.Key.Reference.Key), Has.Member("function_token"));
            Assert.That(blocks.Select((b) => b.Key.Reference.Key), Has.Member("print_token"));

            blocks = lexer.TryNextBlock(language.CreateReference("text"));
            Assert.That(blocks, Has.Count.EqualTo(1));
            Assert.That(blocks[0].Value, Is.EqualTo("hello world"));
        }
    }
}
