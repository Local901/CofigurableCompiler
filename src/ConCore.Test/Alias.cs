using ConCore.Blocks.Helpers;
using ConCore.FileInfo;
using ConCore.Key;
using ConCore.Key.Collections;
using ConCore.Key.Components;
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
                    Language? language = collection.GetLanguage("c_lang");
                    if (language != null)
                    {
                        return language;
                    }

                    language = new Language("test_lang");
                    collection.AddLanguage(language);

                    var tLoad = language.AddKey(new Token("load", "load"));
                    var tPrint = language.AddKey(new Token("print_token", "print"));

                    var tFunction = language.AddKey(new Token("function_token", "[\\S]+"));
                    language.SetAlias(tFunction, tLoad);
                    language.SetAlias(tFunction, tPrint);

                    var tPath = language.AddKey(new Token("path", "[a-zA-Z]+"));
                    var tText = language.AddKey(new Token("text", "[\\S].*$"));

                    var builder = new ComponentBuilder();
                    var cLoadFile = language.AddKey(new Construct("load_file",
                        builder.Ordered(false,
                            builder.Value(tLoad),
                            builder.Value(tPath, "path")
                        )
                    ));

                    var cPrintLine = language.AddKey(new Construct("print_text",
                        builder.Ordered(false,
                            builder.Value(tPrint),
                            builder.Value(tPath)
                        )
                    ));

                    var languageStart = language.AddKey(new Construct("structure",
                        builder.Ordered(false,
                            builder.Repeat(builder.Any(false,
                                builder.Value(cLoadFile),
                                builder.Value(cPrintLine)
                            ))
                        )
                    ));

                    language.StartingKeyReference = languageStart;

                    //language.AddFilter(new FileReference(new BlockReader(), new FileReferenceArgs
                    //{
                    //    KeyReference = cLoadFile,
                    //    ReferenceType = FileReferenceType.Relative,
                    //    ValuePath = new string[] { "path" },
                    //}));

                    return language;
                });
        }

        [Test]
        public void ShouldLexMultipleBlocksAtOnceWhenThereAreAliases()
        {
            var collection = new KeyCollection();
            languageLoaderMock.Object.LoadConfig(null, collection);
            var language = collection.GetLanguage("test_lang");

            var lexer = new SimpleLexer(
                new StreamChunkReader(new StreamReader($"print hello world\nload file")),
                language
                );

            var blocks = lexer.TryNextBlock(language.CreateReference("function_token"));

            Assert.That(blocks, Has.Count.EqualTo(2)); // function_token block and print_token block
            Assert.That(blocks.Select((b) => b.Block.Key.Reference.Key), Has.Member("function_token"));
            Assert.That(blocks.Select((b) => b.Block.Key.Reference.Key), Has.Member("print_token"));

            blocks = lexer.TryNextBlock(language.CreateReference("text"));
            Assert.That(blocks, Has.Count.EqualTo(1));
            Assert.That(blocks[0].Block.Value, Is.EqualTo("hello world"));
        }
    }
}
