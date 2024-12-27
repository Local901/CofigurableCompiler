using ConCore.Blocks;
using ConCore.CustomRegex.Info;
using ConCore.Key;
using ConCore.Key.Components;
using ConCore.Parsing.Simple.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConCore.Parsing.Simple
{
    public class ConstructArgs : ParseArgs, ILocalRoot
    {
        public Construct Key { get; }

        public int Depth { get; }

        public ConstructArgs(Construct key, IValueInfo<bool, Component> component, ILocalRoot? localRoot, IBlock? block)
            : base(component, localRoot, block)
        {
            Key = key;
            Depth = localRoot == null ? 0 : localRoot.Depth + 1;
        }
        public ConstructArgs(Construct key, IValueInfo<bool, Component> component, ILocalRoot? localRoot)
            : this(key, component, localRoot, null) { }

        public override IList<IValueInfo<bool, Component>?> GetNextComponents()
        {
            if (Block == null && Key != null) return Key.Component.DetermainNext(null, true);
            return base.GetNextComponents();
        }

        public IRelationBlock CreateBlock(IParseArgs localArgEnd)
        {
            if (localArgEnd.LocalRoot != this) throw new Exception("The arg end should have this as it's local root.");
            var content = localArgEnd.LocalPath()
                .Select(arg => arg.Block.Copy(arg.Data.Value.Name));

            return new ConstructBlock(Key, content);
        }
    }
}
