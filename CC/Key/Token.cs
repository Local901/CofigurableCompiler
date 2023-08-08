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
        public Token(string key, string pattern, List<Token> subTokens)
            : this(key, pattern)
        {
            SubTokens = subTokens != null ? subTokens : new List<Token>();
            SubTokens.ForEach(t => t.Leader = this);
        }

        public override IKey ProminentKey
        {
            get => Leader == null ? this : Leader.ProminentKey;
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
        public override IKey GetKeyFor(object value)
        {
            if (value == null) return null;
            if (!(value is string)) return null;
            if (Regex.Match((string)value).Value.CompareTo(value) != 0) return null;

            var t = SubTokens.Select(t => t.GetKeyFor(value))
                .FirstOrDefault(input => input != null);
            if (t != null) return t;
            return this;
        }

        public override List<IKey> GetSubKeys()
        {
            return SubTokens.Cast<IKey>().ToList();
        }

        public override List<KeyLangReference> GetSubKeyRefs()
        {
            return SubTokens.Select(t => t.Reference).ToList();
        }
    }
}
