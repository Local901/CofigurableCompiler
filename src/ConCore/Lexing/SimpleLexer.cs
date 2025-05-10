using ConCore.Blocks;
using ConCore.Key;
using ConCore.Key.Collections;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConCore.Lexing
{
    [Flags]
    public enum FileLexerOptions
    {
        Default = 0,
        None = 1,
        // CompleteLanguage = 1,
        // FromAliasRoot = 2,
        ResolveAlias = 4,
    }

    public class SimpleLexer : ILexer
    {
        private ChunkReader Reader;
        private readonly Language Language;
        private readonly FileLexerOptions options;

        /// <summary>
        /// Make lexer for page text.
        /// </summary>
        /// <param name="reader">File reader that returns the first tokens that it comes across.</param>
        /// <param name="language">Collection that contains all the tokens for a language.</param>
        public SimpleLexer(ChunkReader reader, Language language, FileLexerOptions options = FileLexerOptions.Default)
        {
            Reader = reader;
            Language = language;
            this.options = options == FileLexerOptions.Default ? FileLexerOptions.ResolveAlias : options;
        }

        public List<LexResult> TryNextBlock(KeyLangReference key)
        {
            return TryNextBlock(Language.AllChildKeys<Token>(key, true)
                .Select((token) => new TokenArgs
                {
                    Token = token,
                    PrecendingModifier = null,
                    ReadModifier = null,
                })
            );
        }
        public List<LexResult> TryNextBlock(IEnumerable<KeyLangReference> keys)
        {
            return TryNextBlock(keys.SelectMany((key) => Language.AllChildKeys<Token>(key, true))
                .Distinct()
                .Select((token) => new TokenArgs
                {
                    Token = token,
                    PrecendingModifier = null,
                    ReadModifier = null,
                })
            );
        }
        public List<LexResult> TryNextBlock(LexOptions option)
        {
            return TryNextBlock(Language.AllChildKeys<Token>(option.Key, true)
                .Select((token) => new TokenArgs
                {
                    Token = token,
                    PrecendingModifier = option.PrecedingCondition,
                    ReadModifier = null,
                })
            );
        }
        public List<LexResult> TryNextBlock(IEnumerable<LexOptions> options)
        {
            return TryNextBlock(
                options.SelectMany((option) =>
                    Language.AllChildKeys<Token>(option.Key, true)
                        .Select((token) => new TokenArgs
                        {
                            Token = token,
                            PrecendingModifier = option.PrecedingCondition,
                            ReadModifier = null,
                        })
                )
            );
        }
        /// <summary>
        /// Find next block using provided tokens. Will move the progress index to the one that is the higest of the returnd values.
        /// </summary>
        /// <param name="block">Block that gets created.</param>
        /// <param name="tokens">List of tokens.</param>
        /// <returns>Returns true if block is created.</returns>
        private List<LexResult> TryNextBlock(IEnumerable<TokenArgs> tokens)
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
        /// <param name="args">List of tokenArgs.</param>
        /// <returns>Returns true if block is created.</returns>
        private List<LexResult> TryAllBlocks(IEnumerable<TokenArgs> args)
        {
            //// Get all tokens of requested languages.
            //if (options.HasFlag(FileLexerOptions.CompleteLanguage))
            //{
            //    tokens = tokens.GroupBy((t) => t.Token.Reference.Language)
            //        .SelectMany((group) => group.Key.AllKeys<Token>());
            //}
            // Only use root level aliases.
            //if (options.HasFlag(FileLexerOptions.FromAliasRoot))
            //{
            //    var roots = args.SelectMany((t) => t.Token.RootAlliasses())
            //        .Distinct().Cast<Token>().ToList();

            //    var tokenList = args.Where((t) => t.Token.AliasParents.Count() == 0)
            //        .ToList();
            //    tokenList.AddRange(roots.Where((r) => !args.Any((arg) => arg.Token == r)));
            //    args = tokenList;
            //}

            return Reader.NextBlocks(args.ToArray())
                .Select((response) =>
                    new LexResult
                    {
                        Block = new Block(
                            response.Key,
                            response.MatchValue,
                            response.MatchStart,
                            response.MatchEnd
                        ),
                        PrecedingBlock = new Block(
                            null,
                            response.PrecedingValue,
                            response.PrecedingStart,
                            response.MatchStart
                        )
                    })
                .Cast<LexResult>()
                .ToList();
        }

        /// <summary>
        /// Resolve all aliases and return a complete list of all valid blocks.
        /// </summary>
        /// <param name="blocks">list of blocks to resolve.</param>
        /// <returns></returns>
        private IEnumerable<LexResult> ResolveAliasses(IEnumerable<LexResult> blocks)
        {
            return blocks.Select(FindAlias);
        }

        private LexResult FindAlias(LexResult origional)
        {
            IKey? foundAlias = origional.Block.Key;

            if (foundAlias == null)
            {
                return origional;
            }

            var aliases = Language.GetAliases(foundAlias.Reference).ToArray();

            int index = 0;
            while (index < aliases.Length)
            {
                var key = Language.GetKey(aliases[index]);
                if (key == null || key is not Token token)
                {
                    index++;
                    continue;
                }
                var match = token.NextMatch(origional.Block.Value, 0);
                if (match == null || match.Value != origional.Block.Value)
                {
                    index++;
                    continue;
                }
                foundAlias = key;
                aliases = Language.GetAliases(foundAlias.Reference).ToArray();
                index = 0;
            }

            if (foundAlias == origional.Block.Key)
            {
                return origional;
            }

            return new LexResult()
            {
                Block = new Block(foundAlias, origional.Block.Value, origional.Block.Index, origional.Block.EndIndex, origional.Block.Name),
                PrecedingBlock = origional.PrecedingBlock
            };
        }
    }
}
