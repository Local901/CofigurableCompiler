using CC.Blocks;
using CC.Key;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC
{
    public class FileLexer: ILexer
    {
        private string Page;
        private int Index;
        private readonly KeyCollection _tokenCollection;

        /// <summary>
        /// Make lexer for page text.
        /// </summary>
        /// <param name="page">text to lex</param>
        public FileLexer(string page, KeyCollection tokenCollection)
        {
            Page = page;
            Index = 0;
            _tokenCollection = tokenCollection;
        }

        public void SetProgressIndex(int index)
        {
            Index = Math.Max(0, index);
        }
        public void Reset()
        {
            SetProgressIndex(0);
        }

        public bool TryNextBlock(out IBlock block, KeyLangReference key)
        {
            return TryNextBlock(out block, _tokenCollection.GetAllSubKeysOfType<Token>(key, true));
        }
        public bool TryNextBlock(out IBlock block, IEnumerable<KeyLangReference> keys)
        {
            return TryNextBlock(out block, _tokenCollection.GetAllSubKeysOfType<Token>(keys, true));
        }
        /// <summary>
        /// Find next block using provided tokens.
        /// </summary>
        /// <param name="block">Block that gets created.</param>
        /// <param name="tokens">List of tokens.</param>
        /// <returns>Returns true if block is created.</returns>
        private bool TryNextBlock(out IBlock block, IEnumerable<Token> tokens)
        {
            // find the next match
            var match = tokens.Select(t => new
            {
                Match = t.NextMatch(Page, Index),
                Token = t
            })
                .Where(m => m.Match != null)
                .Where(m => m.Match.Value.Length > 0)
                .OrderBy(m => m.Match.Index)
                .FirstOrDefault();

            block = null;
            if (match == null)
                return false;

            // make a block
            block = new Block(
                match.Token,
                match.Match.Value,
                match.Match.Index,
                match.Match.Index + match.Match.Value.Length
            );

            // move last index
            Index = block.EndIndex;
            return true;
        }

        public bool TryAllBlocks(out List<IBlock> blocks, KeyLangReference key)
        {
            return TryAllBlocks(out blocks, _tokenCollection.GetAllSubKeysOfType<Token>(key, true));
        }
        public bool TryAllBlocks(out List<IBlock> blocks, IEnumerable<KeyLangReference> keys)
        {
            return TryAllBlocks(out blocks, _tokenCollection.GetAllSubKeysOfType<Token>(keys, true));
        }
        /// <summary>
        /// Find all blocks using provided tokens.<br/>
        /// <b>It doesn't update the index of the FileLexer.</b>
        /// </summary>
        /// <param name="blocks">Blocks that get created.</param>
        /// <param name="tokens">List of tokens.</param>
        /// <returns>Returns true if block is created.</returns>
        private bool TryAllBlocks(out List<IBlock> blocks, IEnumerable<Token> tokens)
        {
            blocks = tokens.Select(t => new
            {
                Match = t.NextMatch(Page, Index),
                Token = t
            })
                .Where(m => m.Match != null)
                .Where(m => m.Match.Value.Length > 0)
                .Select(m =>
                    new Block(
                        m.Token,
                        m.Match.Value,
                        m.Match.Index,
                        m.Match.Index + m.Match.Value.Length
                    ))
                .Cast<IBlock>()
                .ToList();
            return blocks.Count > 0;
        }
    }
}
