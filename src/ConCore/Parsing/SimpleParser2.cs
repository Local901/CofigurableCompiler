using ConCore.Blocks;
using ConCore.CustomRegex.Info;
using ConCore.CustomRegex.Steps;
using ConCore.Key;
using ConCore.Key.Collections;
using ConCore.Key.Components;
using ConCore.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Parsing
{
    public class SimpleParser2 : IParser
    {
        private readonly ILanguage Language;
        private readonly ILexer Lexer;
        private readonly PreparedLanguage PreparedLanguage;

        public SimpleParser2(ILanguage language, ILexer lexer)
        {
            Language = language;
            Lexer = lexer;
            PreparedLanguage = new PreparedLanguage(language);
        }

        public IBlock? DoParse(KeyLangReference startConstruct)
        {
            return Parse();
        }

        public IBlock? Parse()
        {
            var tokenStack = new List<IBlock>();
            var layerStack = new List<ParseLayer>();
            var botList = new List<Bot>();

            #region Parsing Startup
            {
                IReadOnlyList<KeyLangReference> keys = new KeyLangReference[] { Language.StartingKeyReference };

                ParseLayer? layer = TryGetNextLayer(0, keys);

                if (layer != null)
                {
                    keys = layer.GetStartKeys();
                }

                IList<LexResult> lexResults = Lexer.TryNextBlock(keys);

                // Early exit if StartingKeyReference only relates to tokens.
                if (layer == null)
                {
                    // Return token
                    if (lexResults.Count == 0)
                    {
                        throw new Exception("No token found while parsing.");
                    }
                    return lexResults.First().Block;
                }

                botList = layer.GetBotsForBlocks(Language, lexResults.Select((r) => r.Block).ToList());
            }
            #endregion

            while (true)
            {
                // Get next blocks based on the bots
                var infoValues = botList.SelectMany((bot) => bot.Info.DetermainNext(false));
                bool canEnd = infoValues.Any((info) => info == null);
                var keys = infoValues
                    .Where((info) => info != null)
                    .Select((info) => info.Value.Reference)
                    .Distinct()
                    .ToList();

                // Get next layer if possible.
                var newLayer = TryGetNextLayer(tokenStack.Count, keys);
                if (newLayer != null)
                {
                    // don't push new layer here to the stack (only push if layer is used.)
                    keys.AddRange(newLayer.GetStartKeys());
                }

                var lexResults = Lexer.TryNextBlock(keys);
            }
        }

        /// <summary>
        /// Find the next layer if one of the keys is or contains a constuct.
        /// </summary>
        /// <param name="currentIndex">Current index of the token stack.</param>
        /// <param name="keys">List of keys to search true.</param>
        /// <returns>A new layer if there where any construct keys.</returns>
        public ParseLayer? TryGetNextLayer(int currentIndex, IEnumerable<KeyLangReference> keys)
        {
            var starts = keys.Distinct().SelectMany((key) => PreparedLanguage.GetStarts(key)).Distinct().ToArray();

            // The case of having separate keys that are not constructs will not be taken into account because that is dumb and can be handled by using a construct.
            if (starts.Length == 0)
            {
                return null;
            }
            return new ParseLayer(currentIndex, starts);
        }
    }

    public struct Bot
    {
        public readonly int StartIndex;
        public readonly Construct ParentConstruct;
        public IValueInfo<bool, Component> Info;

        public Bot(int startIndex, Construct construct, IValueInfo<bool, Component> info)
        {
            StartIndex = startIndex;
            ParentConstruct = construct;
            Info = info;
        }
    }

    public class ParseLayer
    {
        /// <summary>
        /// At wich layer of the token stack has this been added.
        /// </summary>
        public readonly int Index;
        public readonly IReadOnlyList<TokenStart> TokenStarts;

        public ParseLayer(int index, IReadOnlyList<TokenStart> starts)
        {
            Index = index;
            TokenStarts = starts;
        }

        public IReadOnlyList<KeyLangReference> GetStartKeys()
        {
            List<KeyLangReference> keys = TokenStarts.SelectMany((s) => s.StartKeys.Keys).ToList();

            var nextStarts = TokenStarts.SelectMany((s) => s.RelatedStarts).ToList();
            List<TokenStart> seenStarts = TokenStarts.ToList();
            bool checkRelated = seenStarts.Count > 0;
            while (checkRelated)
            {
                keys.AddRange(
                    nextStarts.SelectMany((s) => s.StartKeys.Keys)
                        .Where((k) => !keys.Contains(k))
                );
                seenStarts.AddRange(nextStarts);
                nextStarts = nextStarts.SelectMany((s) => s.RelatedStarts)
                    .Where((s) => !seenStarts.Contains(s))
                    .ToList();

                checkRelated = nextStarts.Count > 0;
            }

            return keys.Distinct().ToList();
        }

        public List<Bot> GetBotsForBlocks(ILanguage language, IReadOnlyList<IValueBlock> blocks)
        {
            return blocks.SelectMany((block) =>
            {
                return TokenStarts.SelectMany((start) =>
                {
                    return start.StartKeys
                        .Where((key) => language.IsKeyInGroup(block.Key.Reference, key.Key))
                        .Select((key) => new Bot(Index, start.Construct, key.Value));
                });
            }).ToList();
        }
    }

    public interface ICompletable
    {
        bool CanComplete { get; }
    }

    public class PreparedLanguage
    {
        private Dictionary<KeyLangReference, TokenStart[]> StartDictionary;

        public PreparedLanguage(ILanguage language)
        {
            StartDictionary = new Dictionary<KeyLangReference, TokenStart[]>();

            foreach (Construct construct in language.AllKeys<Construct>())
            {
                StartDictionary.Add(construct.Reference, new TokenStart[] {
                    new TokenStart(language, construct)
                });
            }
            foreach (Token token in language.AllKeys<Token>())
            {
                StartDictionary.Add(token.Reference, new TokenStart[0]);
            }
            foreach (KeyGroup group in language.AllKeys<KeyGroup>())
            {
                StartDictionary.Add(
                    group.Reference,
                    language.AllChildKeys(group.Reference)
                        .SelectMany((childKey) => GetStarts(childKey.Reference))
                        .ToArray()
                );
            }

            // Load all related starts into all construct starts.
            foreach (var start in StartDictionary.Values)
            {
                if (start.Length != 1)
                {
                    continue;
                }
                start[0].LoadRelatedStarts(this);
            }
        }

        /// <summary>
        /// Find all start helper objects for a given key.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>A list of start objects.</returns>
        /// <exception cref="Exception">When no reference was found to the provided key.</exception>
        public TokenStart[] GetStarts(KeyLangReference key)
        {
            TokenStart[] result;
            if (StartDictionary.TryGetValue(key, out result))
            {
                return result;
            }
            throw new Exception($"No key found matching {key} in start dictionary.");
        }

        public bool HasStart(KeyLangReference key)
        {
            return GetStarts(key).Length > 0;
        }
    }

    public class TokenStart: ICompletable
    {
        public readonly Construct Construct;
        public readonly IReadOnlyDictionary<KeyLangReference, IValueInfo<bool, Component>> StartKeys;
        public IReadOnlyList<TokenStart> RelatedStarts { get; private set; }
        public bool CanComplete { get; }

        internal TokenStart(ILanguage language, Construct construct)
        {
            Construct = construct;
            RelatedStarts = new TokenStart[0];

            var start = construct.Component.Start(false);
            if (start.Any((info) => info?.Value.Reference == null))
            {
                CanComplete = true;
            }

            StartKeys = new Dictionary<KeyLangReference, IValueInfo<bool, Component>>(
                start.Where((info) => info?.Value.Reference != null)
                    .Select((info) => new KeyValuePair<KeyLangReference, IValueInfo<bool, Component>>(info.Value.Reference, info))
                    .ToList()
            );
        }

        internal void LoadRelatedStarts(PreparedLanguage preparedLanguage)
        {
            if (RelatedStarts.Count > 0)
            {
                return;
            }
            RelatedStarts = StartKeys.SelectMany((key) => preparedLanguage.GetStarts(key)).ToArray();
        }
    }
}
