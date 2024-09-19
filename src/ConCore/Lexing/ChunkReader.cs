using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Lexing
{
    public interface ChunkReader
    {
        /// <summary>
        /// Get list of blocks that appear first next in the text.
        /// </summary>
        /// <param name="args">Information about the tokens.</param>
        /// <returns>All the results found.</returns>
        BlockReadResult[] NextBlocks(TokenArgs[] args);
    }
}
