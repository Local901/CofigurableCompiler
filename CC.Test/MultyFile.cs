using CC.Blocks;
using CC.FileInfo;
using CC.Key;
using CC.Key.ComponentTypes;
using CC.Key.Modifiers;
using CC.Test.Mock;
using CC.Tools;
using CC.Tools.Contracts;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Test
{
    public class MultyFile
    {
        private Mock<ILanguageLoader> languageLoaderMock;

        [SetUp]
        public void Setup()
        {
            languageLoaderMock = new Mock<ILanguageLoader>();
            languageLoaderMock.Setup((loader) => loader.LoadConfig(It.IsAny<FileData>(), It.IsAny<KeyCollection>()))
                .Returns<FileData, KeyCollection>((file, collection) =>
                {
                    LangCollection language = collection.GetLanguage("test_lang");
                    if (language != null)
                    {
                        return language;
                    }

                    language = new LangCollection("test_lang");
                    collection.AddLanguage(language);

                    var tLoad = language.Add(new Token("load", "load"));
                    var tPrint = language.Add(new Token("print_token", "print"));
                    var tPath = language.Add(new Token("path", "[a-zA-Z]+"));

                    var cLoadFile = language.Add(new Construct("load_file", new OrderComponent(new List<IComponent>
                    {
                        new ValueComponent(tLoad),
                        new ValueComponent(tPath, "path"),
                    })));

                    var cPrintLine = language.Add(new Construct("print_text", new OrderComponent(new List<IComponent>
                    {
                        new ValueComponent(tPrint),
                        new ValueComponent(tPath),
                    })));

                    var languageStart = language.Add(new Construct("structure", new OrderComponent(new List<IComponent>
                    {
                        new RepeatComponent(new ValueComponent(cLoadFile)),
                        new RepeatComponent(new ValueComponent(cPrintLine)),
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
        public void TestShouldOpenTwoFiles()
        {
            string file1 = "load fileTwo";
            string file2 = "print hello";

            int count = 0;
            bool openedFileTwo = false;
            var parser = new MultyFileParserPartialMock(languageLoaderMock.Object, (file) =>
            {
                switch (file.RelativePath)
                {
                    case ("fileOne"):
                        count++;
                        return file1;
                    case ("fileTwo"):
                        count++;
                        openedFileTwo = true;
                        return file2;
                    default:
                        throw new Exception($"Unkown file {file.RelativePath}");
                }
            });

            var result = parser.Parse("fileOne");

            Assert.That(count, Is.EqualTo(2));
            Assert.That(openedFileTwo, Is.True);
            Assert.That(result.ToList(), Has.Count.EqualTo(2));
            foreach(var file in result)
            {
                Assert.That(file.ParsedContent, Is.Not.Null);
            }
        }
    }
}
