using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConCore.Key
{
    public class Token : IKey, IAlias<Token, string>
    {
        private string _pattern;
        private RegexOptions _regexOptions = RegexOptions.Multiline | RegexOptions.Compiled;
        public string Pattern
        {
            get => _pattern;
            set
            {
                _pattern = value;
                MakeRegex();
            }
        }

        public RegexOptions RegexOptions
        {
            get => _regexOptions;
            set
            {
                _regexOptions = value;
                MakeRegex();
            }
        }
        public Regex? Regex { get; private set; }
        public Token Leader { get; private set; }


        private readonly List<IAlias<Token, string>> _aliasses = new();
        public IReadOnlyList<IAlias<Token, string>> Aliasses => _aliasses.ToList();

        private readonly List<IAlias<Token, string>> _aliasParents = new();
        public IReadOnlyList<IAlias<Token, string>> AliasParents => _aliasParents.ToList();

        public Token(string key, string pattern)
        {
            Reference = new KeyLangReference { Key = key };
            Pattern = pattern;
        }

        /// <summary>
        /// Find a macth after index in page.
        /// </summary>
        /// <param name="page">Text to search.</param>
        /// <param name="index">Start index</param>
        /// <returns>Match that was found or null if not found</returns>
        public Match? NextMatch(string page, int index)
        {
            if (Regex == null) return null;
            return Regex.Match(page, index);
        }
        private void MakeRegex()
        {
            if (Pattern == null)
            {
                Regex = null;
            }
            else
            {
                Regex = new Regex(Pattern, RegexOptions);
            }
        }

        public bool Check(string value)
        {
            var match = Regex.Match(value);
            return match.Value == value;
        }

        public void AddAlias(IAlias<Token, string> alias)
        {
            if (alias == null) throw new Exception("Can't add null as an allias.");
            if (_aliasses.Contains(alias)) throw new Exception("Can't add the same alias twice.");
            _aliasses.Add(alias);
            if (!alias.AliasParents.Contains(this)) {
                alias.AddParentAlias(this);
            }
        }

        public void AddParentAlias(IAlias<Token, string> alias)
        {
            if (_aliasParents.Contains(alias)) throw new Exception("Can't add the same parent alias twice.");
            _aliasParents.Add(alias);
            if (!alias.Aliasses.Contains(this))
            {
                alias.AddAlias(this);
            }
        }

        public IKey[] FindAliases(object value, bool includeSelf = true)
        {
            if (value is not string)
            {
                return new IKey[0];
            }
            return FindAliasses((string)value, includeSelf);
        }
        public IKey[] FindAliasses(string value, bool includeSelf = true)
        {
            List<IKey> validAliasses = new List<IKey>();

            if (includeSelf && Check(value))
            {
                validAliasses.Add(this);
            }

            foreach(var alias in _aliasses)
            {
                validAliasses.AddRange(alias.FindAliasses(value));
            }

            return validAliasses.ToArray();
        }

        public bool IsAlias(IAlias allias)
        {
            if (allias is not IAlias<Token, string>)
            {
                return false;
            }
            return IsAlias((IAlias<Token, string>)allias);
        }
        public bool IsAlias(IAlias<Token, string> allias)
        {
            if (allias is Token && Equals(allias as Token))
            {
                return true;
            }

            return Aliasses.Any((a) => a.IsAlias(allias));
        }

        IList<IAlias> IAlias.RootAliases()
        {
            return (IList<IAlias>)RootAlliasses();
        }
        public IList<IAlias<Token, string>> RootAlliasses()
        {
            if (AliasParents.Count == 0)
            {
                return new List<IAlias<Token, string>> { this };
            }

            return AliasParents.SelectMany((parent) => parent.RootAlliasses()).ToList();
        }
    }
}
