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

        public bool Check(IReadOnlyList<IBlock> value)
        {
            throw new NotImplementedException();
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
