using ConCore.Blocks;
using ConCore.Key.Collections;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConCore.FileInfo
{
    public class FileData
    {
        public string RelativePath { get; }
        public string AbsolutePath { get; }

        public List<FileData> Parents { get; }

        public LangCollection Language { get; set; }
        public IBlock ParsedContent { get; set; }

        public FileData(string path) {
            RelativePath = Path.GetRelativePath(".", path);
            AbsolutePath = Path.GetFullPath(path);
            Parents = new List<FileData>();
        }

        public override bool Equals(object obj)
        {
            if (obj is FileData) {
                var fd = obj as FileData;
                return AbsolutePath == fd.AbsolutePath && Language.Language == fd.Language.Language;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AbsolutePath, Language.Language);
        }
    }
}

