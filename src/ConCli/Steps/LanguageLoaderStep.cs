using ConCli.Common;
using ConCli.Langs;
using ConCore.FileInfo;
using ConCore.Key.Collections;
using ConCore.Tools.Contracts;
using ConLine.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCli.Steps
{
    public delegate LangCollection LoadLanguage(PathInstance path, KeyCollection keyCollection);
    public class LanguageLoaderStep : FunctionCallStep<LoadLanguage>
    {
        public LanguageLoaderStep(string name)
            : base(name, new string[] { "keyCollection" })
        {
            FunctionHandler = LoadConfig;
        }

        public LangCollection LoadConfig(PathInstance path, KeyCollection keyCollection)
        {
            var extension = Path.GetExtension(path.OriginalPath);

            LangCollection? language = null;

            if (extension == ".json")
            {
                language = JsonLang.JsonLangConfigLoader(path, keyCollection);
            }

            if (language == null)
            {
                throw new NotImplementedException();
            }
            // Add language to key collection if it
            if (keyCollection.GetLanguage(language.Language) == null)
            {
                keyCollection.AddLanguage(language);
            }
            return language;
        }
    }
}
