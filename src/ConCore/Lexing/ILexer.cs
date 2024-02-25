using ConCore.Blocks;
using ConCore.Key;
using System.Collections.Generic;

namespace ConCore.Lexing
{
    public interface ILexer
    {
        /// <summary>
        /// Restart lexing from the beginning.
        /// </summary>
        void Reset();
        /// <summary>
        /// Manualy update the progress index.
        /// </summary>
        /// <param name="index"></param>
        void SetProgressIndex(int index);

        /// <summary>
        /// Find next block using tokens connected to the key.
        /// Progress is updated.
        /// </summary>
        /// <param name="key">Key of group/token.</param>
        /// <returns>Returns a list of blocks found next. Skipping ones that are found later than the first posible.</returns>
        IList<IValueBlock> TryNextBlock(KeyLangReference key);
        /// <summary>
        /// Find next block using tokens connected to the keys.
        /// Progress is updated.
        /// </summary>
        /// <param name="keys">Keys of groups/tokens.</param>
        /// <returns>Returns a list of blocks found next. Skipping ones that are found later than the first posible.</returns>
        IList<IValueBlock> TryNextBlock(IEnumerable<KeyLangReference> keys);

        /// <summary>
        /// Find next block using tokens connected to the key.<br/>
        /// <b>It doesn't update the progress of the lexer.</b>
        /// </summary>
        /// <param name="key">Key of group/token.</param>
        /// <returns>Returns Blocks that get created.</returns>
        List<IValueBlock> TryAllBlocks(KeyLangReference key);
        /// <summary>
        /// Find next block using tokens connected to the keys.<br/>
        /// <b>It doesn't update the progress of the lexer.</b>
        /// </summary>
        /// <param name="keys">Keys of group/token.</param>
        /// <returns>Returns Blocks that get created.</returns>
        List<IValueBlock> TryAllBlocks(IEnumerable<KeyLangReference> keys);
    }
}
