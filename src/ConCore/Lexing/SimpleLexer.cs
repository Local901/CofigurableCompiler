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
        private TokenReader Reader;
        private readonly Language Language;
        private readonly FileLexerOptions options;

        /// <summary>
        /// Make lexer for page text.
        /// </summary>
        /// <param name="reader">File reader that returns the first tokens that it comes across.</param>
        /// <param name="language">Collection that contains all the tokens for a language.</param>
        public SimpleLexer(TokenReader reader, Language language, FileLexerOptions options = FileLexerOptions.Default)
        {
            Reader = reader;
            Language = language;
            this.options = options == FileLexerOptions.Default ? FileLexerOptions.ResolveAlias : options;
        }

        public LexResult? TryNextBlock(KeyLangReference key)
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
        public LexResult? TryNextBlock(IEnumerable<KeyLangReference> keys)
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
        public LexResult? TryNextBlock(LexOptions option)
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
        public LexResult? TryNextBlock(IEnumerable<LexOptions> options)
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
        private LexResult? TryNextBlock(IEnumerable<TokenArgs> tokens)
        {
            // find the next match
            LexResult? block = ReadBlock(tokens);

            if (!block.HasValue)
            {
                return null;
            }

            LexResult result = block.Value;

            // Create all blocks of valid child aliases.
            if (options.HasFlag(FileLexerOptions.ResolveAlias))
            {
                result = ResolveAliasses(result);
            }

            return result;
        }

        /// <summary>
        /// Find all blocks using provided tokens.<br/>
        /// <b>It doesn't update the index of the FileLexer.</b>
        /// </summary>
        /// <param name="blocks">Blocks that get created.</param>
        /// <param name="args">List of tokenArgs.</param>
        /// <returns>Returns true if block is created.</returns>
        private LexResult? ReadBlock(IEnumerable<TokenArgs> args)
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

            BlockReadResult? optionalResult = Reader.NextBlock(args.ToArray());

            if (!optionalResult.HasValue)
            {
                return null;
            }

            BlockReadResult result = optionalResult.Value;

            return new LexResult
            {
                Block = new Block(
                    result.Key,
                    result.MatchValue,
                    result.MatchStart,
                    result.MatchEnd
                ),
                PrecedingBlock = new Block(
                    null,
                    result.PrecedingValue,
                    result.PrecedingStart,
                    result.MatchStart
                )
            };
        }

        /// <summary>
        /// Resolve all aliases and return a lex result that matches the first deepest alias.
        /// </summary>
        /// <param name="origional">Result to resolve.</param>
        /// <returns>Lex result mapped to its alias.</returns>
        private LexResult ResolveAliasses(LexResult origional)
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
