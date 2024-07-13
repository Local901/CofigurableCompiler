using ConCore.FileInfo;
using ConCore.Key.Collections;
using ConCore.Tools.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCli
{
    public class LanguageLoader : ILanguageLoader
    {
        public LangCollection LoadConfig(FileData file, KeyCollection keyCollection)
        {
            var extension = Path.GetExtension(file.RelativePath);

            if (extension == ".json")
            {

            }

            throw new NotImplementedException();
        }
    }
}
