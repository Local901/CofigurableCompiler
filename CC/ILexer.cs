using CC.Blocks;
using CC.Key;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC
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
        /// <param name="block">Block that gets created.</param>
        /// <param name="key">Key of group/token.</param>
        /// <returns>Returns true if block is created.</returns>
        bool TryNextBlock(out IBlock block, KeyLangReference key);
        /// <summary>
        /// Find next block using tokens connected to the keys.
        /// Progress is updated.
        /// </summary>
        /// <param name="block">Block that gets created.</param>
        /// <param name="keys">Keys of groups/tokens.</param>
        /// <returns>Returns true if block is created.</returns>
        bool TryNextBlock(out IBlock block, IEnumerable<KeyLangReference> keys);

        /// <summary>
        /// Find next block using tokens connected to the key.<br/>
        /// <b>It doesn't update the progress of the lexer.</b>
        /// </summary>
        /// <param name="blocks">Blocks that get created.</param>
        /// <param name="key">Key of group/token.</param>
        /// <returns>Returns true if block is created.</returns>
        bool TryAllBlocks(out List<IBlock> blocks, KeyLangReference key);
        /// <summary>
        /// Find next block using tokens connected to the keys.<br/>
        /// <b>It doesn't update the progress of the lexer.</b>
        /// </summary>
        /// <param name="blocks">Blocks that get created.</param>
        /// <param name="keys">Keys of group/token.</param>
        /// <returns>Returns true if block is created.</returns>
        bool TryAllBlocks(out List<IBlock> blocks, IEnumerable<KeyLangReference> keys);
    }
}
