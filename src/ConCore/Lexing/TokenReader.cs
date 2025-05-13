using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Lexing
{
    public interface TokenReader
    {
        /// <summary>
        /// Get the block that appears next in the text.
        /// </summary>
        /// <param name="args">Information about the tokens.</param>
        /// <returns>The result found.</returns>
        BlockReadResult? NextBlock(TokenArgs[] args);
    }
}
