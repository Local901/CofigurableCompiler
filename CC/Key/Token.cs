using BranchList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CC.Key
{
    public class Token : IKey, IAliased<Token, string>
    {
        private string _pattern;
        private RegexOptions _regexOptions = RegexOptions.None;
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
        public Regex Regex { get; private set; }
        public Token Leader { get; private set; }


        private readonly List<IAliased<Token, string>> _aliasses;
        public IReadOnlyList<IAliased<Token, string>> Aliasses => _aliasses.ToList();

        private readonly List<IAliased<Token, string>> _aliasParents;
        public IReadOnlyList<IAliased<Token, string>> AliasParents => _aliasParents.ToList();

        public Token(string key, string pattern)
        {
            Reference = new KeyLangReference { Key = key };
            Pattern = pattern;
            _aliasses = new List<IAliased<Token, string>>();
            _aliasParents = new List<IAliased<Token, string>>();
        }

        /// <summary>
        /// Find a macth after index in page.
        /// </summary>
        /// <param name="page">Text to search.</param>
        /// <param name="index">Start index</param>
        /// <returns>Match that was found or null if not found</returns>
        public Match NextMatch(string page, int index)
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

        public void AddAlias(IAliased<Token, string> alias)
        {
            if (_aliasses.Contains(alias)) throw new Exception("Can't add the same alias twice.");
            _aliasses.Add(alias);
            if (!alias.AliasParents.Contains(this)) {
                alias.AddParentAlias(this);
            }
        }

        public void AddParentAlias(IAliased<Token, string> alias)
        {
            if (_aliasParents.Contains(alias)) throw new Exception("Can't add the same parent alias twice.");
            _aliasParents.Add(alias);
            if (!alias.Aliasses.Contains(this))
            {
                alias.AddAlias(this);
            }
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
    }
}
