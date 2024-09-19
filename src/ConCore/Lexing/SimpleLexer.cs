using ConCore.Blocks;
using ConCore.Key;
using ConCore.Key.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConCore.Lexing
{
    [Flags]
    public enum FileLexerOptions
    {
        None = 0,
        CompleteLanguage = 1,
        FromAliasRoot = 2,
        ResolveAlias = 4,
    }

    public class SimpleLexer : ILexer
    {
        private ChunkReader Reader;
        private readonly KeyCollection _tokenCollection;
        private readonly FileLexerOptions options;

        /// <summary>
        /// Make lexer for page text.
        /// </summary>
        /// <param name="reader">File reader that returns the first tokens that it comes across.</param>
        /// <param name="tokenCollection">Collection that contains all loaded languages and tokens.</param>
        public SimpleLexer(ChunkReader reader, KeyCollection tokenCollection, FileLexerOptions options = FileLexerOptions.None)
        {
            Reader = reader;
            _tokenCollection = tokenCollection;
            this.options = options == FileLexerOptions.None ? FileLexerOptions.ResolveAlias : options;
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
            // find the next match
            var blocks = TryAllBlocks(tokens);

            if (blocks.Count == 0)
            {
                return blocks;
            }

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

            return Reader.NextBlocks(tokens.Select((token) =>
                    new TokenArgs {
                        Token = token,
                        PrecendingModifier = null,
                        ReadModifier = null
                    })
                    .ToArray())
                .Select((response) =>
                    new Block(
                        response.Key,
                        response.MatchValue,
                        response.MatchStart.Index,
                        response.MatchEnd.Index
                    ))
                .Cast<IValueBlock>()
                .ToList();
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
                // Keys of ValueBlocks are always tokens.
                var token = (Token)block.Key;

                return token.FindAliasses(block.Value, false)
                    .Select((t) => new Block(t, block.Value, block.Index, block.EndIndex, block.Name))
                    .Prepend(block);
            });
        }
    }
}
