using CC.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CC.Lexing
{
    public class Token : IKey
    {
        private string _pattern;
        private bool _isMultiline;
        private bool _ignoreCase;
        private bool _ignoreWhiteSpace;
        private bool _isRightToLeft;
        public string Pattern 
        {
            get => _pattern;
            set
            {
                _pattern = value;
                MakeRegex();
            }
        }
        public bool IsMultiline
        {
            get => _isMultiline;
            set
            {
                _isMultiline = value;
                MakeRegex();
            }
        }
        public bool IgnoreCase
        {
            get => _ignoreCase;
            set
            {
                _ignoreCase = value;
                MakeRegex();
            }
        }
        public bool IgnoreWhiteSpace
        {
            get => _ignoreWhiteSpace;
            set
            {
                _ignoreWhiteSpace = value;
                MakeRegex();
            }
        }
        public bool IsRightToLeft
        {
            get => _isRightToLeft;
            set
            {
                _isRightToLeft = value;
                MakeRegex();
            }
        }
        public Regex Regex { get; private set; }
        public List<Token> SubTokens { get; }

        public Token()
        {
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
                Regex = new Regex(Pattern, GetOptions());
            }
        }
        private RegexOptions GetOptions()
        {
            RegexOptions ro = RegexOptions.None;
            if (IsMultiline)
            {
                ro |= RegexOptions.Multiline;
            }
            if (IgnoreCase)
            {
                ro |= RegexOptions.IgnoreCase;
            }
            if (IgnoreWhiteSpace)
            {
                ro |= RegexOptions.IgnorePatternWhitespace;
            }
            if (IsRightToLeft)
            {
                ro |= RegexOptions.RightToLeft;
            }
            return ro;
        }
        public override IKey GetKey(object value)
        {
            if (!(value is string)) return null;
            string v = (string)value;
            if (!(Regex.Match(v).Value.CompareTo(value) == 0)) return null;

            var t = SubTokens.Select(t => t.GetKey(value))
                .FirstOrDefault(input => input != null);
            if (t != null) return t;
            return this;
        }

        public override List<IKey> GetKeys()
        {
            return SubTokens.Cast<IKey>().ToList();
        }
    }
}
