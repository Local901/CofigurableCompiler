using ConCore.Blocks;
using ConCore.CustomRegex.Info;
using ConCore.Key;
using ConCore.Key.Collections;
using ConCore.Key.Components;
using ConCore.Lexing;
using ConCore.Parsing.Simple.Stack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Parsing.Simple
{
    public class ParentInfo
    {
        /// <summary>
        /// Parent construct info.
        /// </summary>
        public readonly ConstructInfo Parent;
        /// <summary>
        /// Value info that resulted in the relation.
        /// </summary>
        public readonly IValueInfo<bool, Component> RelatedInfo;

        public ParentInfo(ConstructInfo parent, IValueInfo<bool, Component> relatedInfo)
        {
            Parent = parent;
            RelatedInfo = relatedInfo;
        }
    }
    public class ConstructInfo
    {
        /// <summary>
        /// Construct key.
        /// </summary>
        public readonly Construct Construct;
        /// <summary>
        /// Start values. No instant construct end (null).
        /// </summary>
        public readonly IValueInfo<bool, Component>[] StartValues;
        /// <summary>
        /// Parent construct info.
        /// </summary>
        public readonly List<ParentInfo> Parents;

        public ConstructInfo(Construct construct)
        {
            Construct = construct;
            StartValues = construct.Component.Start(false).Where((v) => v != null).ToArray();
            Parents = new List<ParentInfo>();
        }
        public ConstructInfo(Construct construct, ParentInfo parent) : this(construct)
        {
            Parents.Add(parent);
        }
    }
    public class ConstructReferenceInfo
    {
        /// <summary>
        /// Reference that resulted on the provided info.
        /// </summary>
        public readonly KeyLangReference OrigionalReference;
        /// <summary>
        /// All the construct info.
        /// </summary>
        public readonly ConstructInfo[] Constructs;
        /// <summary>
        /// All construct info resulted directly from the oribional refernece.
        /// </summary>
        public readonly ConstructInfo[] StartConstructs;

        public ConstructReferenceInfo(KeyLangReference origionalReference, ConstructInfo[] constructs, ConstructInfo[] startConstructs)
        {
            OrigionalReference = origionalReference;
            Constructs = constructs;
            StartConstructs = startConstructs;
        }

        public static ConstructReferenceInfo CreateFrom(ILanguage language, KeyLangReference origionalReference)
        {
            var startConstructs = language.AllChildKeys<Construct>(origionalReference, true)
                .Select((construct) => new ConstructInfo(construct))
                .ToArray();
            var allConstructs = startConstructs.ToList();

            int index = 0;
            while (index < allConstructs.Count)
            {
                ConstructInfo info = allConstructs[index];
                foreach(IValueInfo<bool, Component> value in info.StartValues)
                {
                    foreach(Construct construct in language.AllChildKeys<Construct>(value.Value.Reference, true))
                    {
                        ConstructInfo? childInfo = allConstructs.FirstOrDefault((i) => i.Construct == construct);
                        var relation = new ParentInfo(info, value);
                        if (childInfo != null)
                        {
                            childInfo.Parents.Add(relation);
                            continue;
                        }
                        allConstructs.Add(new ConstructInfo(construct, relation));
                    }
                }
                index++;
            }
            return new ConstructReferenceInfo(origionalReference, allConstructs.ToArray(), startConstructs);
        }
    }

    public class LayerInstance : BotCreator, ILayer
    {
        public override Construct? Construct { get; }
        public override ILayer? Layer { get; }
        public IValueInfo<bool, Component>? Info { get; }
        public ParseStack<IBlock>.StackInterface TokenReference { get; }
        private readonly ConstructReferenceInfo LayerInfo;

        private LayerInstance(
            ParseStack<IBlock>.StackInterface stackInterface,
            IValueInfo<bool, Component> info,
            ILayer? layer,
            Construct? construct,
            ILanguage language
        ) {
            TokenReference = stackInterface;
            Info = info;
            Layer = layer;
            Construct = construct;

            LayerInfo = ConstructReferenceInfo.CreateFrom(language, info.Value.Reference);
        }

        public LayerInstance(
            ParseStack<IBlock>.StackInterface stackInterface,
            IValueInfo<bool, Component> info,
            Construct construct,
            ILayer layer,
            ILanguage language
        ) : this(stackInterface, info, layer, construct, language) { }
        public LayerInstance(
            ParseStack<IBlock>.StackInterface stackInterface,
            IValueInfo<bool, Component> info,
            ILanguage language
        ) : this(stackInterface, info, default(ILayer), null, language) { }

        public IEnumerable<IBot> GetBots(ILanguage language, ParseStack<IBlock>.StackInterface stackInterface)
        {
            return LayerInfo.Constructs.SelectMany(
                (info) => info.StartValues.SelectMany(
                    (value) => CreateBot(language, stackInterface, value, this, info.Construct, false)
                )
            );
        }

        public IEnumerable<IBot> DetermainNext(ILanguage language, ParseStack<IBlock> stack, IBot childBot)
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
                    foreach (var bot in CreateBot(language, nextReference, value, this, parent.Parent.Construct, false))
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
                foreach (var bot in CreateBot(language, nextReference, value))
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
                    foreach (IBot bot in Layer.DetermainNext(language, stack, endBot))
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
    }
}
