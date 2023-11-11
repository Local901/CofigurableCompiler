
using System.Collections.Generic;
using System.IO;
using CC.Blocks;
using CC.Key;

namespace CC.FileInfo
{
    public class FileData
    {
        public string RelativePath { get; }
        public string AbsolutePath { get; }

        public List<FileData> Parents { get; }

        public KeyLangReference LanguageStart { get; set; }
        public IBlock ParsedContent { get; set; }

        public FileData(string path) {
            RelativePath = Path.GetRelativePath(".", path);
            AbsolutePath = Path.GetFullPath(path);
            Parents = new List<FileData>();
        }
    }
}

