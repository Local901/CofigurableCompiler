using BranchList;
using CC.Blocks;
using CC.Key;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CC
{
    public class MultyFileParser<TLexer, TParser> : IMultiFileParser
        where TLexer : class, ILexer
        where TParser : class, IParser
    {
        private ILanguageLoader[] languageLoaders;

        public MultyFileParser(ILanguageLoader[] languageLoaders)
        {
            this.languageLoaders = languageLoaders;
        }

        public IBlock[] Parse(string filePath)
        {
            var keyCollection = new KeyCollection();
            languageLoaders.ForEach(loader => loader.LoadLanguage(keyCollection));

            throw new NotImplementedException();
        }

        private ILexer CreateLexer(string fileContent, KeyCollection keyCollection)
        {
            List<object> parameters = null;
            var LexerConstructor = typeof(TLexer).GetConstructors().FirstOrDefault(conster =>
            {
                parameters = new List<object>();
                return conster.GetParameters().All(par => {
                    var parType = par.ParameterType;
                    if (parType.IsAssignableFrom(typeof(string)))
                    {
                        parameters.Add(fileContent);
                    }
                    else if (parType.IsAssignableFrom(typeof(KeyCollection)))
                    {
                        parameters.Add(keyCollection);
                    }
                    else
                    {
                        return false;
                    }
                    return true;
                });
            });

            if (LexerConstructor == null)
                throw new NullReferenceException("No valid constructor was found for " + typeof(TLexer).ToString());

            return LexerConstructor.Invoke(parameters.ToArray()) as ILexer;
        }

        private IParser CreateParser(string fileContent, KeyCollection keyCollection)
        {
            ILexer lexer = CreateLexer(fileContent, keyCollection);

            List<object> parameters = null;
            var parserConstructor = typeof(TLexer).GetConstructors().FirstOrDefault(conster =>
            {
                parameters = new List<object>();
                return conster.GetParameters().All(par => {
                    var parType = par.ParameterType;
                    if (parType.IsAssignableFrom(typeof(ILexer)))
                    {
                        parameters.Add(lexer);
                    }
                    else if (parType.IsAssignableFrom(typeof(KeyCollection)))
                    {
                        parameters.Add(keyCollection);
                    }
                    else
                    {
                        return false;
                    }
                    return true;
                });
            });

            if (parserConstructor == null)
                throw new NullReferenceException("No valid constructor was found for " + typeof(TParser).ToString());

            return parserConstructor.Invoke(parameters.ToArray()) as IParser;
        }
    }
}
