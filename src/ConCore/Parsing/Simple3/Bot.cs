using ConCore.Blocks;
using ConCore.CustomRegex.Info;
using ConCore.Key;
using ConCore.Key.Collections;
using ConCore.Key.Components;
using ConCore.Lexing;
using ConCore.Parsing.Simple3.Stack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Parsing.Simple3
{
    public enum CheckPreceding
    {
        NotUsed = 0,
        Correct = 1,
        Incorrect = 2,
    }

    public class Bot : BotCreator, IBot
    {
        public override Construct? Construct { get; }

        public override ILayer? Layer { get; }

        public ParseStack<IBlock>.StackInterface TokenReference { get; }

        public IValueInfo<bool, Component>? Info { get; }

        protected Bot(ParseStack<IBlock>.StackInterface stackInterface, IValueInfo<bool, Component>? info, ILayer? layer, Construct? construct)
        {
            Construct = construct;
            Layer = layer;
            TokenReference = stackInterface;
            Info = info;
        }

        public Bot(ParseStack<IBlock>.StackInterface stackInterface, IValueInfo<bool, Component>? info, Construct construct, ILayer layer)
            : this(stackInterface, info, layer, construct) { }
        public Bot(ParseStack<IBlock>.StackInterface stackInterface, IValueInfo<bool, Component>? info)
            : this(stackInterface, info, default(ILayer), null) { }

        public virtual IEnumerable<IBot> DetermainNext(ILanguage language, ParseStack<IBlock> stack, IReadOnlyList<LexResult> lexResults)
        {
            // If it is a end placeholder bot yield emediatly.
            if (Info == null) yield break;

            foreach (LexResult result in lexResults)
            {
                // Skip for all that don't match.
                if (result.Block.Key?.Reference == null || !language.IsKeyInGroup(result.Block.Key?.Reference, Info.Value.Reference))
                {
                    continue;
                }
                var hasPreceding = CheckPrecedingBlock(result.PrecedingBlock);
                if (hasPreceding == CheckPreceding.Incorrect)
                {
                    continue;
                }

                // Push blocks
                ParseStack<IBlock>.StackInterface nextReference = TokenReference;
                if (hasPreceding == CheckPreceding.Correct)
                {
                    nextReference = nextReference.Add(result.PrecedingBlock);
                }
                nextReference = nextReference.Add(result.Block.Copy(Info.Value.Name));

                // Create bots for next round.
                var nextInfos = Info.DetermainNext(false);

                // Make bots for next values
                foreach (IValueInfo<bool, Component>? info in nextInfos)
                {
                    if (info == null) continue;
                    foreach (IBot bot in CreateBot(language, nextReference, info))
                    {
                        yield return bot;
                    }
                }

                // This step could be the last of this construct.
                if (nextInfos.Contains(null))
                {
                    var endBot = new EndedBot(nextReference, Layer, Construct);
                    if (Layer != null)
                    {
                        // If a parent layer is found try to determain next of the parent layer.
                        foreach (IBot bot in Layer.DetermainNext(language, stack, endBot))
                        {
                            yield return bot;
                        }
                    }
                    else
                    {
                        // Without layer just return the ended bot.
                        yield return endBot;
                    }
                }
            }
            yield break;
        }

        protected virtual CheckPreceding CheckPrecedingBlock(IValueBlock precedingBlock)
        {
            // TODO: If a preceding value/filter thingy is added to component. Add a check to just say yes.

            return CheckPreceding.NotUsed;
        }

        public virtual IEnumerable<LexOptions> GetLexOptions(ILanguage language)
        {
            if (Info == null) yield break;
            yield return new LexOptions(Info.Value.Reference);
        }
    }
}
