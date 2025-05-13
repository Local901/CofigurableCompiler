using ConCore.Blocks;
using ConCore.Key;
using System.Collections.Generic;
using ConCore.Lexing.Errors;

namespace ConCore.Lexing
{
    public interface ILexer
    {
        /// <summary>
        /// Find next block using tokens connected to the key.
        /// Progress is updated.
        /// </summary>
        /// <param name="key">Key of group/token.</param>
        /// <exception cref="NoNextBlockFound"></exception>
        /// <returns>>Returns next block found.</returns>
        LexResult? TryNextBlock(KeyLangReference key);
        /// <summary>
        /// Find next block using tokens connected to the keys.
        /// Progress is updated.
        /// </summary>
        /// <param name="keys">Keys of groups/tokens.</param>
        /// <exception cref="NoNextBlockFound"></exception>
        /// <returns>>Returns next block found.</returns>
        LexResult? TryNextBlock(IEnumerable<KeyLangReference> keys);
        /// <summary>
        /// Find next block using tokens connected to the key.
        /// Progress is updated.
        /// </summary>
        /// <param name="options">Options for finding a token.</param>
        /// <exception cref="NoNextBlockFound"></exception>
        /// <returns>>Returns next block found.</returns>
        LexResult? TryNextBlock(LexOptions options);
        /// <summary>
        /// Find next block using tokens connected to the keys.
        /// Progress is updated.
        /// </summary>
        /// <param name="optionsList">List op options for finding keys.</param>
        /// <exception cref="NoNextBlockFound"></exception>
        /// <returns>Returns next block found.</returns>
        LexResult? TryNextBlock(IEnumerable<LexOptions> optionsList);
    }
}
