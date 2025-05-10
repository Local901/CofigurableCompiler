using ConCore.CustomRegex.Info;
using ConCore.Key.Collections;
using ConCore.Key;
using ConCore.Parsing.Simple3.Stack;
using ConCore.Parsing.Simple3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Key.Components;
using ConCore.Lexing;

namespace ConCore.Blocks
{
    public abstract class BotCreator
    {
        public abstract Construct? Construct { get; }
        public abstract ILayer? Layer { get; }

        protected virtual IEnumerable<IBot> CreateBot(ILanguage language, ParseStack<IBlock>.StackInterface nextReference, IValueInfo<bool, Component> nextInfo)
        {
            return CreateBot(language, nextReference, nextInfo, Layer, Construct);
        }
        protected virtual IEnumerable<IBot> CreateBot(
            ILanguage language,
            ParseStack<IBlock>.StackInterface nextReference,
            IValueInfo<bool, Component> nextInfo,
            ILayer? layer,
            Construct? construct,
            bool onlyBot = false
        )
        {
            if (!onlyBot && language.AllChildKeys<Construct>(nextInfo.Value.Reference, true).Count() > 0)
            {
                foreach (IBot bot in new LayerInstance(nextReference, nextInfo, construct, layer, language).GetBots(language, nextReference))
                {
                    yield return bot;
                }
            }
            if (language.AllChildKeys<Token>(nextInfo.Value.Reference, true).Count() > 0)
            {
                yield return new Bot(nextReference, nextInfo, construct, layer);
            }
        }
    }
}
