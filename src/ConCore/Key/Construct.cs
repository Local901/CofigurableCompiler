using ConCore.Blocks;
using ConCore.Key.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConCore.Key

{
    public class Construct : IKey, IAlias<Construct, IReadOnlyList<IBlock>>
    {
        public Component Component { get; }

        private readonly List<IAlias<Construct, IReadOnlyList<IBlock>>> _aliasses = new();
        public IReadOnlyList<IAlias<Construct, IReadOnlyList<IBlock>>> Aliasses => _aliasses.ToList();

        private readonly List<IAlias<Construct, IReadOnlyList<IBlock>>> _aliasParents = new();
        public IReadOnlyList<IAlias<Construct, IReadOnlyList<IBlock>>> AliasParents => _aliasParents.ToList();

        public Construct (string key, Component component)
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

        public void AddAlias(IAlias<Construct, IReadOnlyList<IBlock>> alias)
        {
            if (_aliasses.Contains(alias)) throw new Exception("Can't add the same alias twice.");
            _aliasses.Add(alias);
            if (!alias.AliasParents.Contains(this))
            {
                alias.AddParentAlias(this);
            }
        }

        public void AddParentAlias(IAlias<Construct, IReadOnlyList<IBlock>> alias)
        {
            if (_aliasParents.Contains(alias)) throw new Exception("Can't add the same parent alias twice.");
            _aliasParents.Add(alias);
            if (!alias.Aliasses.Contains(this))
            {
                alias.AddAlias(this);
            }
        }

        public IKey[] FindAliases(object value, bool includeSelf = true)
        {
            if (!(value is IReadOnlyList<IBlock>))
            {
                return new IKey[0];
            }
            return FindAliasses(value as IReadOnlyList<IBlock>, includeSelf);
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

        public bool IsAlias(IAlias allias)
        {
            if (!(allias is IAlias<Construct, IReadOnlyList<IBlock>>))
            {
                return false;
            }
            return IsAlias(allias as IAlias<Construct, IReadOnlyList<IBlock>>);
        }
        public bool IsAlias(IAlias<Construct, IReadOnlyList<IBlock>> allias)
        {
            if (allias is Construct && Equals(allias as Construct))
            {
                return true;
            }

            return Aliasses.Any((a) => a.IsAlias(allias));
        }

        IList<IAlias> IAlias.RootAliases()
        {
            return RootAlliasses() as IList<IAlias>;
        }
        public IList<IAlias<Construct, IReadOnlyList<IBlock>>> RootAlliasses()
        {
            if (AliasParents.Count == 0)
            {
                return new List<IAlias<Construct, IReadOnlyList<IBlock>>> { this };
            }

            return AliasParents.SelectMany((parent) => parent.RootAlliasses()).ToList();
        }
    }
}
