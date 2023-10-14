using BranchList;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CC.Key
{
    public class Token : IKey
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
        public IReadOnlyList<Token> SubTokens { get; }

        public Token(string key, string pattern)
        {
            Reference = new KeyLangReference { Key = key };
            Pattern = pattern;
            SubTokens = new List<Token>();
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
    }
}
