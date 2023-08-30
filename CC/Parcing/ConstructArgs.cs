using BranchList;
using CC.Blocks;
using CC.Key;
using CC.Key.ComponentTypes;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace CC.Parcing
{
    public class ConstructArgs : ParseArgs, ILocalRoot
    {
        public IConstruct Key { get; }

        public int Depth { get; }

        public ConstructArgs(IConstruct key, IValueComponentData component, ILocalRoot localRoot, IBlock block)
            : base(component, localRoot, block)
        {
            Key = key;
            Depth = localRoot.Depth + 1;
        }
        public ConstructArgs(IConstruct key, IValueComponentData component, ILocalRoot localRoot)
            : this(key, component, localRoot, null) { }

        public override IList<IValueComponentData> GetNextComponents()
        {
            if (Block == null) return Key.Component.GetNextComponents(null);
            return base.GetNextComponents();
        }

        public IRelationBlock CreateBlock(IParseArgs localArgEnd)
        {
            if (localArgEnd.LocalRoot != this) throw new Exception("The arg end should have this as it's local root.");
            var content = localArgEnd.LocalPath()
                .Select(arg => arg.Block.Copy(arg.Data.Component.Name));

            return new ConstructBlock(Key, content);
        }
    }
}
