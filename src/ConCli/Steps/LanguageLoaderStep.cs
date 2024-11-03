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
    public delegate Language LoadLanguage(PathInstance path, KeyCollection keyCollection);
    public class LanguageLoaderStep : FunctionCallStep<LoadLanguage>
    {
        public LanguageLoaderStep(string name)
            : base(name, new string[] { "keyCollection" })
        {
            FunctionHandler = LoadConfig;
        }

        public Language LoadConfig(PathInstance path, KeyCollection keyCollection)
        {
            var extension = Path.GetExtension(path.OriginalPath);

            Language? language = null;

            if (extension == ".json")
            {
                language = JsonLang.JsonLangConfigLoader(path, keyCollection);
            }

            if (language == null)
            {
                throw new NotImplementedException();
            }
            // Add language to key collection if it
            if (keyCollection.GetLanguage(language.Name) == null)
            {
                keyCollection.AddLanguage(language);
            }
            return language;
        }
    }
}
