using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConCore.Key
{
    public class Token : IKey
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
        public Regex Regex { get; private set; }
        public Token Leader { get; private set; }


        public Token(string key, string pattern)
            : base(key)
        {
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
    }
}
