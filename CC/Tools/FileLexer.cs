using CC.Blocks;
using CC.Key;
using CC.Tools.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Tools
{
    [Flags]
    public enum FileLexerOptions
    {
        None = 0,
        CompleteLanguage = 1,
        FromAliasRoot = 2,
        ResolveAlias = 4,
    }

    public class FileLexer: ILexer
    {
        private string Page;
        private int Index;
        private readonly KeyCollection _tokenCollection;
        private readonly FileLexerOptions options;

        /// <summary>
        /// Make lexer for page text.
        /// </summary>
        /// <param name="page">text to lex</param>
        public FileLexer(string page, KeyCollection tokenCollection, FileLexerOptions options = FileLexerOptions.None)
        {
            Page = page;
            Index = 0;
            _tokenCollection = tokenCollection;
            this.options = options == FileLexerOptions.None ? FileLexerOptions.ResolveAlias : options;
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

            // Take all first blocks
            blocks = blocks.Where((b) => b.Index == minIndex).ToList();

            // Create all blocks of valid child aliases.
            if (options.HasFlag(FileLexerOptions.ResolveAlias))
            {
                blocks = ResolveAliasses(blocks).ToList();
            }

            // move to last index
            SetProgressIndex(blocks.Select((b) => b.EndIndex).Max());
            return blocks;
        }

        public List<IValueBlock> TryAllBlocks(KeyLangReference key)
        {
            var blocks = TryAllBlocks(_tokenCollection.GetAllSubKeysOfType<Token>(key, true));

            // Create all blocks of valid child aliases.
            if (options.HasFlag(FileLexerOptions.ResolveAlias))
            {
                blocks = ResolveAliasses(blocks).ToList();
            }

            return blocks;
        }
        public List<IValueBlock> TryAllBlocks(IEnumerable<KeyLangReference> keys)
        {
            var blocks = TryAllBlocks(_tokenCollection.GetAllSubKeysOfType<Token>(keys, true));

            // Create all blocks of valid child aliases.
            if (options.HasFlag(FileLexerOptions.ResolveAlias))
            {
                blocks = ResolveAliasses(blocks).ToList();
            }

            return blocks;
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
            // Get all tokens of requested languages.
            if (options.HasFlag(FileLexerOptions.CompleteLanguage))
            {
                tokens = tokens.GroupBy((t) => t.Reference.Language)
                    .SelectMany((group) => group.Key.GetAllKeysOfType<Token>());
            }
            // Only use root level aliases.
            if (options.HasFlag(FileLexerOptions.FromAliasRoot))
            {
                var roots = tokens.SelectMany((t) => t.RootAlliasses())
                    .Distinct().Cast<Token>().ToList();

                var tokenList = tokens.Where((t) => t.AliasParents.Count() == 0)
                    .ToList();
                tokenList.AddRange(roots.Where((r) => !tokens.Contains(r)));
                tokens = tokenList;
            }

            var blocks = tokens.Select(t => new
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

            return blocks;
        }

        /// <summary>
        /// Resolve all aliases and return a complete list of all valid blocks.
        /// </summary>
        /// <param name="blocks">list of blocks to resolve.</param>
        /// <returns></returns>
        private IEnumerable<IValueBlock> ResolveAliasses(IEnumerable<IValueBlock> blocks)
        {
            return blocks.SelectMany((block) =>
            {
                var token = block.Key as Token;

                return token.FindAliasses(block.Value, false)
                    .Select((t) => new Block(t, block.Value, block.Index, block.EndIndex, block.Name))
                    .Prepend(block);
            });
        }
    }
}
