using CC.Blocks;
using CC.Key;
using CC.Tools.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Tools
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

        public IList<IValueBlock> TryNextBlock(KeyLangReference key)
        {
            return TryNextBlock(_tokenCollection.GetAllSubKeysOfType<Token>(key, true));
        }
        public IList<IValueBlock> TryNextBlock(IEnumerable<KeyLangReference> keys)
        {
            return TryNextBlock(_tokenCollection.GetAllSubKeysOfType<Token>(keys, true));
        }
        /// <summary>
        /// Find next block using provided tokens. Will move the progress index to the one that is the higest of the returnd values.
        /// </summary>
        /// <param name="block">Block that gets created.</param>
        /// <param name="tokens">List of tokens.</param>
        /// <returns>Returns true if block is created.</returns>
        private IList<IValueBlock> TryNextBlock(IEnumerable<Token> tokens)
        {
            // TODO: Make it optional to search using only the root aliases instead of the 

            // find the next match
            var blocks = TryAllBlocks(tokens);

            if (blocks.Count == 0)
            {
                return blocks;
            }

            var minIndex = blocks.Select((b) => b.Index).Min();

            // Find all valid aliases
            blocks = blocks.Where((b) => b.Index == minIndex)
                .SelectMany((b) =>
                {
                    var alias = b.Key as IAlias;
                    return alias?.FindAliasses(b.Value, false)
                        ?.Select((key) => new Block(key, b.Value, b.Index, b.EndIndex))
                        ?.Prepend(b) ?? new IValueBlock[] { b };
                }).ToList();

            // move to last index
            SetProgressIndex(blocks.Select((b) => b.EndIndex).Max());
            return blocks;
        }

        public List<IValueBlock> TryAllBlocks(KeyLangReference key)
        {
            return TryAllBlocks(_tokenCollection.GetAllSubKeysOfType<Token>(key, true));
        }
        public List<IValueBlock> TryAllBlocks(IEnumerable<KeyLangReference> keys)
        {
            return TryAllBlocks(_tokenCollection.GetAllSubKeysOfType<Token>(keys, true));
        }
        /// <summary>
        /// Find all blocks using provided tokens.<br/>
        /// <b>It doesn't update the index of the FileLexer.</b>
        /// </summary>
        /// <param name="blocks">Blocks that get created.</param>
        /// <param name="tokens">List of tokens.</param>
        /// <returns>Returns true if block is created.</returns>
        private List<IValueBlock> TryAllBlocks(IEnumerable<Token> tokens)
        {
            return tokens.Select(t => new
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
                .Cast<IValueBlock>()
                .ToList();
        }
    }
}
