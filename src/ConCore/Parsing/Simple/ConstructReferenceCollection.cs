using ConCore.CustomRegex.Info;
using ConCore.Key.Collections;
using ConCore.Key;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Key.Components;

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
                foreach (IValueInfo<bool, Component> value in info.StartValues)
                {
                    foreach (Construct construct in language.AllChildKeys<Construct>(value.Value.Reference, true))
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

    public class ConstructReferenceCollection
    {
        private IReadOnlyDictionary<KeyLangReference, ConstructReferenceInfo?> References;

        public ConstructReferenceCollection(ILanguage language)
        {
            References = new Dictionary<KeyLangReference, ConstructReferenceInfo?>(
                language.AllKeys().Select((key) => key.Reference)
                    .Select((key) => new KeyValuePair<KeyLangReference, ConstructReferenceInfo?>(key, CreateReference(language, key)))
            );
        }

        public ConstructReferenceInfo? GetReference(KeyLangReference key)
        {
            ConstructReferenceInfo? info = null;
            References.TryGetValue(key, out info);
            return info;
        }

        private ConstructReferenceInfo? CreateReference(ILanguage language, KeyLangReference origionalReference)
        {
            var startConstructs = language.AllChildKeys<Construct>(origionalReference, true)
                .Select((construct) => new ConstructInfo(construct))
                .ToArray();
            var allConstructs = startConstructs.ToList();

            if (allConstructs.Count == 0) {
                return null;
            }

            int index = 0;
            while (index < allConstructs.Count)
            {
                ConstructInfo info = allConstructs[index];
                foreach (IValueInfo<bool, Component> value in info.StartValues)
                {
                    foreach (Construct construct in language.AllChildKeys<Construct>(value.Value.Reference, true))
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
}
