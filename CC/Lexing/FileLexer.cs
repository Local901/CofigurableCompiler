using CC.Contract;
using CC.Grouping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Lexing
{
    public class FileLexer
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

        /// <summary>
        /// Find next block using all tokens.
        /// </summary>
        /// <param name="block">Block that gets created.</param>
        /// <returns>Returns true if block is created.</returns>
        public bool TryNextBlock(out IBlock block)
        {
            return TryNextBlock(out block, _tokenCollection.GetAllKeysOfType<Token>());
        }
        /// <summary>
        /// Find next block using tokens connected to the key.
        /// </summary>
        /// <param name="block">Block that gets created.</param>
        /// <param name="key">Key of group/token.</param>
        /// <returns>Returns true if block is created.</returns>
        public bool TryNextBlock(out IBlock block, string key)
        {
            return TryNextBlock(out block, _tokenCollection.GetMemberKeysOfType<Token>(key));
        }
        /// <summary>
        /// Find next block using tokens connected to the keys.
        /// </summary>
        /// <param name="block">Block that gets created.</param>
        /// <param name="keys">Keys of group/token.</param>
        /// <returns>Returns true if block is created.</returns>
        public bool TryNextBlock(out IBlock block, IEnumerable<string> keys)
        {
            return TryNextBlock(out block, _tokenCollection.GetMemberKeysOfType<Token>(keys));
        }
        /// <summary>
        /// Find next block using provided tokens.
        /// </summary>
        /// <param name="block">Block that gets created.</param>
        /// <param name="tokens">List of tokens.</param>
        /// <returns>Returns true if block is created.</returns>
        public bool TryNextBlock(out IBlock block, List<Token> tokens)
        {
            // find the next match
            var match = tokens.Select(t => new
            {
                Match = t.NextMatch( Page, Index),
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
            block = new Block
            {
                Key = match.Token.GetKeyFor(match.Match.Value),
                Index = match.Match.Index,
                EndIndex = match.Match.Index + match.Match.Value.Length,
                Value = match.Match.Value
            };

            // move last index
            Index = block.EndIndex;
            return true;
        }

        /// <summary>
        /// Find all blocks using all tokens.<br/>
        /// <b>It doesn't update the index of the FileLexer.</b>
        /// </summary>
        /// <param name="blocks">Blocks that get created.</param>
        /// <returns>Returns true if block is created.</returns>
        public bool TryAllBlocks(out List<IBlock> blocks)
        {
            return TryAllBlocks(out blocks, _tokenCollection.GetAllKeysOfType<Token>());
        }
        /// <summary>
        /// Find next block using tokens connected to the key.<br/>
        /// <b>It doesn't update the index of the FileLexer.</b>
        /// </summary>
        /// <param name="blocks">Blocks that get created.</param>
        /// <param name="key">Key of group/token.</param>
        /// <returns>Returns true if block is created.</returns>
        public bool TryAllBlocks(out List<IBlock> blocks, string key)
        {
            return TryAllBlocks(out blocks, _tokenCollection.GetMemberKeysOfType<Token>(key));
        }
        /// <summary>
        /// Find next block using tokens connected to the keys.<br/>
        /// <b>It doesn't update the index of the FileLexer.</b>
        /// </summary>
        /// <param name="blocks">Blocks that get created.</param>
        /// <param name="keys">Keys of group/token.</param>
        /// <returns>Returns true if block is created.</returns>
        public bool TryAllBlocks(out List<IBlock> blocks, IEnumerable<string> keys)
        {
            return TryAllBlocks(out blocks, _tokenCollection.GetMemberKeysOfType<Token>(keys));
        }
        /// <summary>
        /// Find all blocks using provided tokens.<br/>
        /// <b>It doesn't update the index of the FileLexer.</b>
        /// </summary>
        /// <param name="blocks">Blocks that get created.</param>
        /// <param name="tokens">List of tokens.</param>
        /// <returns>Returns true if block is created.</returns>
        public bool TryAllBlocks(out List<IBlock> blocks, List<Token> tokens)
        {
            blocks = tokens.Select(t => new
            {
                Match = t.NextMatch(Page, Index),
                Token = t
            })
                .Where(m => m.Match != null)
                .Where(m => m.Match.Value.Length > 0)
                .Select(m =>
                    new Block
                    {
                        Key = m.Token.GetKeyFor(m.Match.Value),
                        Index = m.Match.Index,
                        EndIndex = m.Match.Index + m.Match.Value.Length,
                        Value = m.Match.Value
                    })
                .Cast<IBlock>()
                .ToList();
            return blocks.Count > 0;
        }

        /// <summary>
        /// Lex whole page with these tokens
        /// </summary>
        /// <param name="page"></param>
        /// <param name="tokens"></param>
        /// <returns>A block filled with lexed blocks</returns>
        public static List<IBlock> LexAll(string page, List<Token> tokens)
        {
            var matches = tokens.Select(t => t.Regex.Matches(page).Where(m => !String.IsNullOrEmpty(m.Value))
            .Select(m => new { Token = t, Match = m }));

            int index = 0;
            List<IBlock> program = new List<IBlock>();
            bool doLoop = true;
            while (doLoop)
            {
                // select next match
                var nextMatch = matches.Select(a => a.FirstOrDefault(m => m.Match.Index >= index))
                    .OrderBy(m => m.Match.Index)
                    .FirstOrDefault();

                if (nextMatch == null)
                {
                    doLoop = false;
                    continue;
                }

                // make block
                Block next = new Block
                {
                    Key = nextMatch.Token,
                    Index = nextMatch.Match.Index,
                    EndIndex = nextMatch.Match.Index + nextMatch.Match.Value.Length,
                    Value = nextMatch.Match.Value
                };

                index = next.EndIndex;
                program.Add(next);
            }

            return program;
        }
    }
}
