using BranchList;
using CC.Blocks;
using CC.Key;
using CC.Key.ComponentTypes;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CC.Parcing
{
    public abstract class ParseArgs : TypeBranchNode<ParseArgs, IParseArgs>, IParseArgs
    {
        public ParseStatus Status { get; protected set; }

        public ValueComponent Component { get; }

        public ILocalRoot LocalRoot { get; }

        public IBlock Block { get; protected set; }

        public ParseArgs(ValueComponent component, ILocalRoot localRoot)
            : base()
        {
            Status = ParseStatus.None;
            Component = component;
            LocalRoot = localRoot;
        }

        public virtual ParseStatus UseBlock(IBlock block, KeyCollection keyCollection, IParseArgFactory factory)
        {
            if (Block != null) throw new Exception($"This parse step already contains a block");

            if (!keyCollection.GetLanguage(Component.Reference.Lang).IsKeyInGroup(block.Key.Reference, Component.Reference))
            {
                // Set Status to Error
                Status |= ParseStatus.Error;
                block = new ErrorBlock(block, keyCollection.GetKey(Component.Reference));
            }
            Block = block.Copy(Component.Name);

            var components = Component.GetNextComponents();
            if (components.Any(comp => comp == null))
            {
                Status |= ParseStatus.CanEnd;
            }

            components = components.Where(comp => comp != null).ToList();
            if (components.Count() == 0)
            {
                Status |= ParseStatus.ReachedEnd;
            }

            // When a error has been found check next components for a posible match to the block.
            if (Status.HasFlag(ParseStatus.Error))
            {
                var nextComponents = components.Where(comp => keyCollection.GetLanguage(Component.Reference.Lang).IsKeyInGroup(Block.Key.Reference, comp.Reference)).ToList();
                var args = nextComponents.SelectMany(comp => factory.CreateArg(comp, LocalRoot));
                AddRange(args);
                args.ForEach(arg => arg.UseBlock(block, keyCollection, factory));
            }

            AddRange(components.SelectMany(comp => factory.CreateArg(comp, LocalRoot)));

            return Status;
        }

        public List<IParseArgs> LocalPath()
        {
            return Path(parent => parent == LocalRoot);
        }

        public void RemoveBranch()
        {
            if (Parent != null)
            {
                if (Parent.Children.Count() > 1)
                {
                    Parent.Remove(this);
                }
                else
                {
                    Parent.RemoveBranch();
                }
            }
        }
    }
}
