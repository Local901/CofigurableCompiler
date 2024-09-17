using ConCore.Key;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Lexing.Errors
{
    /// <summary>
    /// Exception class for a lexer.
    /// </summary>
    public class LexException : Exception
    {
        public static readonly string DEFAULT_MESSAGE = "Failed to lex.";

        public readonly IReadOnlyList<KeyLangReference> RequestedKeys;

        public LexException(IReadOnlyList<KeyLangReference> requestedKeys)
            :this(requestedKeys, DEFAULT_MESSAGE) { }

        public LexException(IReadOnlyList<KeyLangReference> requestedKeys, string? message)
            :base(message ?? DEFAULT_MESSAGE)
        {
            RequestedKeys = requestedKeys;
        }
    }
}
