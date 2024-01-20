using CC.Blocks;
using CC.Key.ComponentTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CC.Key

{
    public class Construct : IKey, IAliased<Construct, IReadOnlyList<IBlock>>
    {
        public IComponent Component { get; }

        private readonly List<IAliased<Construct, IReadOnlyList<IBlock>>> _aliasses;
        public IReadOnlyList<IAliased<Construct, IReadOnlyList<IBlock>>> Aliasses => _aliasses.ToList();

        private readonly List<IAliased<Construct, IReadOnlyList<IBlock>>> _aliasParents;
        public IReadOnlyList<IAliased<Construct, IReadOnlyList<IBlock>>> AliasParents => _aliasParents.ToList();

        public Construct (string key, IComponent component)
        {
            Reference = new KeyLangReference { Key = key };
            Component = component;
        }

        public bool Check(IReadOnlyList<IBlock> values)
        {
            var language = Reference.Language;
            var data = Component.GetNextComponents(null);

            foreach (var value in values)
            {
                // Only use data that corresponds with the possible data.
                data = data.Where((d) => language.IsKeyInGroup(value.Key.Reference, d.Component.Reference)).ToList();

                // If no data remains it is not of this type.
                if (data.Count == 0)
                {
                    return false;
                }

                // Get next data.
                data = data.SelectMany((d) => d.GetNextComponents()).ToList();
            }

            return true;
        }

        public void AddAlias(IAliased<Construct, IReadOnlyList<IBlock>> alias)
        {
            if (_aliasses.Contains(alias)) throw new Exception("Can't add the same alias twice.");
            _aliasses.Add(alias);
            if (!alias.AliasParents.Contains(this))
            {
                alias.AddParentAlias(this);
            }
        }

        public void AddParentAlias(IAliased<Construct, IReadOnlyList<IBlock>> alias)
        {
            if (_aliasParents.Contains(alias)) throw new Exception("Can't add the same parent alias twice.");
            _aliasParents.Add(alias);
            if (!alias.Aliasses.Contains(this))
            {
                alias.AddAlias(this);
            }
        }

        public IKey[] FindAliasses(IReadOnlyList<IBlock> value, bool includeSelf = true)
        {
            List<IKey> validAliasses = new List<IKey>();

            if (includeSelf && Check(value))
            {
                validAliasses.Add(this);
            }

            foreach (var alias in _aliasses)
            {
                validAliasses.AddRange(alias.FindAliasses(value));
            }

            return validAliasses.ToArray();
        }
    }
}
