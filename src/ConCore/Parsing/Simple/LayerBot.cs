using ConCore.Blocks;
using ConCore.CustomRegex.Info;
using ConCore.Key;
using ConCore.Key.Collections;
using ConCore.Key.Components;
using ConCore.Key.Conditions;
using ConCore.Lexing;
using ConCore.Parsing.Simple.Stack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Parsing.Simple
{
    public class LayerBot : BotCreator, IBot, ILayer
    {
        public override Construct? Construct { get; }
        public override ILayer? Layer { get; }
        public override IValueInfo<bool, Component> Info { get; }
        public ParseStack<IBlock>.StackInterface TokenReference { get; }
        private readonly ConstructReferenceInfo LayerInfo;

        private LayerBot(
            ParseStack<IBlock>.StackInterface stackInterface,
            IValueInfo<bool, Component> info,
            ILayer? layer,
            Construct? construct,
            ConstructReferenceInfo layerInfo
        )
        {
            TokenReference = stackInterface;
            Info = info;
            Layer = layer;
            Construct = construct;

            LayerInfo = layerInfo;
        }

        public LayerBot(
            ParseStack<IBlock>.StackInterface stackInterface,
            IValueInfo<bool, Component> info,
            Construct construct,
            ILayer layer,
            ConstructReferenceInfo layerInfo
        ) : this(stackInterface, info, layer, construct, layerInfo) { }
        public LayerBot(
            ParseStack<IBlock>.StackInterface stackInterface,
            IValueInfo<bool, Component> info,
            ConstructReferenceInfo layerInfo
        ) : this(stackInterface, info, default(ILayer), null, layerInfo) { }

        public IEnumerable<IBot> GetBots(ILanguage language, ParseStack<IBlock>.StackInterface stackInterface, ConstructReferenceCollection referenceCollection)
        {
            return LayerInfo.Constructs.SelectMany(
                (info) => info.StartValues.SelectMany(
                    (value) => CreateBot(language, referenceCollection, stackInterface, value, this, info.Construct, false)
                )
            );
        }

        public IEnumerable<IBot> DetermainNext(ILanguage language, ParseStack<IBlock> stack, ConstructReferenceCollection referenceCollection, IBot childBot)
        {
            var relatedInfo = LayerInfo.Constructs.FirstOrDefault((i) => i.Construct == childBot.Construct);
            if (relatedInfo == null)
            {
                throw new Exception($"No related info for {childBot.Construct}");
            }
            bool isFirstInfo = LayerInfo.StartConstructs.Contains(relatedInfo);

            var newBlock = new ConstructBlock(relatedInfo.Construct, stack.GetBetween(TokenReference, childBot.TokenReference));

            // push block
            var nextReference = TokenReference.Add(newBlock.Copy(Info.Value.Name));

            // Propogate to parent construct
            foreach (ParentInfo parent in relatedInfo.Parents)
            {
                foreach (IValueInfo<bool, Component>? value in parent.RelatedInfo.DetermainNext(false))
                {
                    if (value == null)
                    {
                        continue;
                    }
                    foreach (var bot in CreateBot(language, referenceCollection, nextReference, value, this, parent.Parent.Construct, false))
                    {
                        yield return bot;
                    }
                }
            }

            if (!isFirstInfo)
            {
                yield break;
            }

            var nextValues = Info.DetermainNext(false).ToArray();

            // Make bots for next values
            foreach (IValueInfo<bool, Component>? value in nextValues)
            {
                if (value == null)
                {
                    continue;
                }
                foreach (var bot in CreateBot(language, referenceCollection, nextReference, value))
                {
                    yield return bot;
                }
            }

            // Layer is last in its layer
            if (nextValues.Contains(null))
            {
                var endBot = new EndedBot(nextReference, Layer, relatedInfo.Construct);
                if (Layer != null)
                {
                    // If a parent layer is found try to determain next of the parent layer.
                    foreach (IBot bot in Layer.DetermainNext(language, stack, referenceCollection, endBot))
                    {
                        yield return bot;
                    }
                }
                else
                {
                    yield return endBot;
                }
            }

            yield break;
        }

        public IEnumerable<IBot> DetermainNext(
            ILanguage language,
            ParseStack<IBlock> stack,
            ConstructReferenceCollection referenceCollection,
            LexResult lexResults
        )
        {
            // If it is a end placeholder bot yield emediatly.
            if (Info == null) yield break;

            // Skip for all that don't match.
            if (lexResults.Block.Key?.Reference == null)
            {
                yield break;
            }
            var hasPreceding = CheckPrecedingBlock(lexResults.PrecedingBlock);
            if (hasPreceding == CheckPreceding.Incorrect)
            {
                yield break;
            }

            // Push blocks
            ParseStack<IBlock>.StackInterface nextReference = TokenReference;
            if (hasPreceding == CheckPreceding.Correct)
            {
                nextReference = nextReference.Add(lexResults.PrecedingBlock.Copy(Info!.Value.PrecedingOptions?.Name));
            }
            nextReference = nextReference.Add(lexResults.Block.Copy(Info!.Value.Name));

            foreach (var constructInfo in LayerInfo.Constructs)
            {
                var nextInfos = constructInfo.StartValues
                    .Where((info) => language.IsKeyInGroup(info.Value.Reference, lexResults.Block.Key.Reference))
                    .SelectMany((info) => info.DetermainNext(false))
                    .ToList();

                foreach(var info in nextInfos)
                {
                    if (info == null) continue;
                    foreach (IBot bot in CreateBot(language, referenceCollection, nextReference, info, this, constructInfo.Construct))
                    {
                        yield return bot;
                    }
                }

                // This step could be the last of this construct.
                if (nextInfos.Contains(null))
                {
                    var endBot = new EndedBot(nextReference, this, constructInfo.Construct);
                    // This layer is the parent layer
                    foreach (IBot bot in DetermainNext(language, stack, referenceCollection, endBot))
                    {
                        yield return bot;
                    }
                }
            }
        }

        private ReadCondition? GetReadCondition(PrecedingOptions? parentOptions, PrecedingOptions? childOptions)
        {
            var options = parentOptions ?? childOptions ?? null;
            return options?.TemplateCondition.Build(new ConditionArgs());
        }

        public IEnumerable<LexOptions> GetLexOptions(ILanguage language)
        {
            return LayerInfo.Constructs.SelectMany((cInfo) => cInfo.StartValues)
                .Select((info) => new LexOptions(
                    info.Value.Reference,
                    GetReadCondition(Info.Value.PrecedingOptions, info.Value.PrecedingOptions)
                ));
        }
    }
}
