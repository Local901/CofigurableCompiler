using ConCore.Key;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Lexing.Errors
{
    /// <summary>
    /// An exception when the lexer can lex a next block with the provided keys.
    /// </summary>
    public class NoNextBlockFound : LexException
    {
        public new static readonly string DEFAULT_MESSAGE = "Not able to lex a block for the requested keys.";

        public NoNextBlockFound(IReadOnlyList<KeyLangReference> requestedKeys)
            :base(requestedKeys, DEFAULT_MESSAGE) { }
    }
}
