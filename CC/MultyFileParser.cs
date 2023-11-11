using BranchList;
using CC.Blocks;
using CC.FileInfo;
using CC.Key;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CC
{
    public class MultyFileParser : IMultiFileParser
    {
        private ILanguageLoader languageLoader;

        public MultyFileParser(ILanguageLoader languageLoader)
        {
            this.languageLoader = languageLoader;
        }

        public FileData[] Parse(string filePath)
        {
            var keyCollection = new KeyCollection();
            var index = 0;
            var fileList = new List<FileData> {
                new FileData(filePath)
            };

            while (fileList.Count() > index) {
                var file = fileList[index];
                languageLoader.LoadConfig(file, keyCollection);

                try {
                    var lexer = CreateLexer(file, keyCollection);
                    var parser = CreateParser(lexer, keyCollection);

                    IBlock block;
                    parser.DoParse(out block, file.LanguageStart);

                    file.ParsedContent = block;

                    // Find all file relations.
                    GetAllFileReferences(file)
                        .ForEach((path) => {
                            fileList.Add(new FileData(path));
                        });

                } catch (Exception) {}
            }

            return fileList.ToArray();
        }

        protected virtual ILexer CreateLexer(FileData file, KeyCollection keyCollection)
        {
            var fileContent = File.ReadAllText(file.AbsolutePath);
            return new FileLexer(fileContent, keyCollection);
        }

        protected virtual IParser CreateParser(ILexer lexer, KeyCollection keyCollection)
        {
            return new FileParser(lexer, keyCollection);
        }

        protected virtual string[] GetAllFileReferences(FileData file) {
            throw new NotImplementedException();
        }
    }
}
