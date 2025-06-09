using ConCore.CustomRegex.Info;
using ConCore.Key.Collections;
using ConCore.Key;
using ConCore.Parsing.Simple.Stack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Key.Components;
using ConCore.Lexing;
using ConCore.Blocks;
using ConCore.Key.Conditions;

namespace ConCore.Parsing.Simple
{
    public abstract class BotCreator
    {
        public abstract Construct? Construct { get; }
        public abstract ILayer? Layer { get; }
        public abstract IValueInfo<bool, Component>? Info { get; }

        protected virtual IEnumerable<IBot> CreateBot(ILanguage language, ConstructReferenceCollection referenceCollection, ParseStack<IBlock>.StackInterface nextReference, IValueInfo<bool, Component> nextInfo)
        {
            return CreateBot(language, referenceCollection, nextReference, nextInfo, Layer, Construct);
        }
        protected virtual IEnumerable<IBot> CreateBot(
            ILanguage language,
            ConstructReferenceCollection referenceCollection,
            ParseStack<IBlock>.StackInterface nextReference,
            IValueInfo<bool, Component> nextInfo,
            ILayer? layer,
            Construct? construct,
            bool onlyBot = false
        )
        {
            var reference = referenceCollection.GetReference(nextInfo.Value.Reference);
            if (!onlyBot && reference != null)
            {
                yield return new LayerBot(nextReference, nextInfo, construct, layer, reference);
            }
            if (language.AllChildKeys<Token>(nextInfo.Value.Reference, true).Count() > 0)
            {
                yield return new Bot(nextReference, nextInfo, construct, layer);
            }
        }

        protected virtual CheckPreceding CheckPrecedingBlock(IValueBlock precedingBlock)
        {
            var precedingOptions = Info?.Value.PrecedingOptions;
            if (precedingOptions == null)
            {
                return CheckPreceding.NotUsed;
            }

            // TODO: manage conditionArgs
            ReadCondition condition = precedingOptions.TemplateCondition.Build(new ConditionArgs());

            if (condition.IsMatch(precedingBlock.Value))
            {
                return CheckPreceding.Correct;
            }

            return CheckPreceding.Incorrect;
        }
    }
}
