using BranchList;
using CC.Blocks;
using CC.FileInfo;
using CC.Key;
using CC.Key.Modifiers;
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

                var languageStartKey = file.Language.FindFilter<LanguageStart>().FindKey(file.Language);

                try {
                    var lexer = CreateLexer(file, keyCollection);
                    var parser = CreateParser(lexer, keyCollection);

                    IBlock block;
                    parser.DoParse(out block, languageStartKey.Reference);

                    file.ParsedContent = block;

                    // Find all file relations.
                    GetAllFileReferences(file)
                        .ForEach((newFile) => {
                            var sameFiles = fileList.FindAll((f) => f.AbsolutePath == newFile.AbsolutePath);
                            if (sameFiles.Count == 0)
                            {
                                fileList.Add(newFile);
                            }
                            else
                            {
                                sameFiles.ForEach((f) => f.Parents.AddRange(newFile.Parents));
                            }
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

        /// <summary>
        /// Get all files that are related to the file.
        /// </summary>
        /// <param name="file">The file that has already been parsed.</param>
        /// <returns>A list of related files.</returns>
        protected virtual FileData[] GetAllFileReferences(FileData file) {
            throw new NotImplementedException();
        }
    }
}
