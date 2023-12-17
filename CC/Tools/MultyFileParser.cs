using BranchList;
using CC.Blocks;
using CC.FileInfo;
using CC.Key;
using CC.Key.Modifiers;
using CC.Tools.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CC.Tools
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
            var parsedFileList = new List<FileData>();
            var fileList = new Queue<FileData>();

            fileList.Enqueue(new FileData(filePath));

            while (fileList.Count() > 0) {
                var file = fileList.Dequeue();
                languageLoader.LoadConfig(file, keyCollection);

                // skip files that have already been parsed
                FileData simmilar;
                if ((simmilar = parsedFileList.FirstOrDefault((f) => f.Equals(file))) != null) {
                    // Link Parents to the similar file.
                    simmilar.Parents.AddRange(file.Parents.Where((p) => simmilar.Parents.Contains(p)));
                    continue;
                }

                var languageStartKey = file.Language.FindFilter<LanguageStart>().FindKey();

                try {
                    var lexer = CreateLexer(file, keyCollection);
                    var parser = CreateParser(lexer, keyCollection);

                    IBlock block;
                    parser.DoParse(out block, languageStartKey.Reference);

                    file.ParsedContent = block;

                    // Find all file relations.
                    GetAllFileReferences(file)
                        .ForEach((newFile) => {
                            fileList.Enqueue(newFile);
                        });

                } catch (Exception) {}

                parsedFileList.Add(file);
            }

            return parsedFileList.ToArray();
        }

        /// <summary>
        /// Create a Lexer.
        /// </summary>
        /// <param name="file">The file that should be lexed.</param>
        /// <param name="keyCollection">The keyCollection with all loaded keys.</param>
        /// <returns>A Lexer.</returns>
        protected virtual ILexer CreateLexer(FileData file, KeyCollection keyCollection)
        {
            var fileContent = File.ReadAllText(file.AbsolutePath);
            return new FileLexer(fileContent, keyCollection);
        }

        /// <summary>
        /// Create a Parser.
        /// </summary>
        /// <param name="lexer">The lexer to use for parsing.</param>
        /// <param name="keyCollection">The keyCollecction with all loaded keys.</param>
        /// <returns>A Parser.</returns>
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
            return file.Language
                .FindFilters<FileReference>()
                .SelectMany((filter) => filter.FindFileReferences(file.ParsedContent))
                .Where((path) => path != null)
                .Select((path) => new FileData(path))
                .ToArray();
        }
    }
}
