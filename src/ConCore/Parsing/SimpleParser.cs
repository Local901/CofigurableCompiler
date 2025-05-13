using ConCore.Blocks;
using ConCore.CustomRegex.Info;
using ConCore.CustomRegex.Steps;
using ConCore.Key;
using ConCore.Key.Collections;
using ConCore.Key.Components;
using ConCore.Lexing;
using ConCore.Parsing.Simple;
using ConCore.Parsing.Simple.Stack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Parsing
{
    public class SimpleParser : IParser
    {
        private readonly ILanguage Language;
        private readonly ILexer Lexer;

        public SimpleParser(ILanguage language, ILexer lexer)
        {
            Language = language;
            Lexer = lexer;
        }

        public IBlock? DoParse(KeyLangReference startConstruct)
        {
            return Parse(startConstruct);
        }

        public IBlock? Parse()
        {
            return Parse(Language.StartingKeyReference);
        }

        public IBlock? Parse(KeyLangReference startRef)
        {
            ParseStack<IBlock> stack = new LinkedParseStack<IBlock>();

            // Make initial bots
            List<IBot> bots = Language.AllChildKeys(startRef, true)
                .SelectMany((key) =>
                {
                    if (key is Token token)
                    {
                        return new IBot[] {
                            new Bot(
                                stack.GetRoot(),
                                new ValueInfo<bool, Component>(new Component(key.Reference))
                            )};
                    }
                    if (key is Construct construct)
                    {
                        return new LayerInstance(
                            stack.GetRoot(),
                            new ValueInfo<bool, Component>(new Component(key.Reference)),
                            Language
                        ).GetBots(Language, stack.GetRoot());
                    }
                    return new IBot[0];
                })
                .Where((key) => key != null)
                .ToList();
            // All bots that can end.
            List<EndedBot> endBots = new List<EndedBot>();
            List<ILayer?> layers = GetLayers(bots);

            // Get tokens.
            var lexOptions = bots.SelectMany((bot) => bot.GetLexOptions(Language)).ToArray();
            var lexResults = Lexer.TryNextBlock(lexOptions);

            while (lexResults.Count != 0)
            {
                var nextBots = bots.SelectMany((bot) => bot.DetermainNext(Language, stack, lexResults)).ToList();
                var nextEndBots = nextBots.OfType<EndedBot>().ToList();
                var nextLayers = GetLayers(nextBots);
                var missingLayers = layers.Where((layer) => !nextLayers.Contains(layer)).ToList();

                // Prep next round.
                bots = nextBots;
                lexOptions = bots.SelectMany((bot) => bot.GetLexOptions(Language)).ToArray();
                lexResults = Lexer.TryNextBlock(lexOptions);
                endBots = nextEndBots;
                layers = nextLayers;
            }

            return endBots.FirstOrDefault()?.TokenReference.Item;
        }

        private List<ILayer?> GetLayers(IEnumerable<IBot> bots)
        {
            return bots.Where((bot) => !(bot is EndedBot))
                .Select((bot) => bot.Layer)
                .Distinct()
                .ToList();
        }
    }
}
