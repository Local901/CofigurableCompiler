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
    public class ParseArgs : TypeBranchNode<ParseArgs, IParseArgs>, IParseArgs
    {
        public ParseStatus Status { get; set; }

        public IValueComponentData Data { get; }

        public ILocalRoot LocalRoot { get; }

        public IBlock Block { get; protected set; }

        public ParseArgs(IValueComponentData data, ILocalRoot localRoot, IBlock block)
            : this(data, localRoot)
        {
            Block = block;
        }
        public ParseArgs(IValueComponentData data, ILocalRoot localRoot)
            : base()
        {
            Status = ParseStatus.None;
            Data = data;
            LocalRoot = localRoot;
        }

        public virtual IList<IValueComponentData> GetNextComponents()
        {
            return Data.GetNextComponents();
        }

        public virtual ParseStatus SetBlock(IBlock block, KeyCollection keyCollection, IParseArgFactory factory)
        {
            if (Block != null) throw new Exception($"This parse step already contains a block");

            if (!keyCollection.GetLanguage(Data.Component.Reference.Lang).IsKeyInGroup(block.Key.Reference, Data.Component.Reference))
            {
                // Set Status to Error
                Status |= ParseStatus.Error;
                block = new ErrorBlock(block, keyCollection.GetKey(Data.Component.Reference));
            }
            Block = block.Copy(Data.Component.Name);

            var components = Data.GetNextComponents();
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
                var nextComponents = components.Where(comp => keyCollection.GetLanguage(Data.Component.Reference.Lang).IsKeyInGroup(Block.Key.Reference, comp.Component.Reference)).ToList();
                var args = nextComponents.SelectMany(comp => factory.CreateArg(comp.Component, LocalRoot));
                AddRange(args);
                args.ForEach(arg => arg.SetBlock(block, keyCollection, factory));
            }

            AddRange(components.SelectMany(comp => factory.CreateArg(comp.Component, LocalRoot)));

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
