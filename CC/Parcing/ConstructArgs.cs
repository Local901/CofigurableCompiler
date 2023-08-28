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

        private ConstructArgs(IConstruct key, IValueComponentData component, ILocalRoot localRoot, IBlock block)
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

        public override ParseStatus SetBlock(IBlock block, KeyCollection keyCollection, IParseArgFactory factory)
        {
            base.SetBlock(block, keyCollection, factory);

            var lastBlock = LastBlock(Block);
            if (lastBlock is ErrorBlock)
            {   // try to use the last block if it was an errorblock.
                ErrorBlock errorBlock = lastBlock as ErrorBlock;

                Ends().Where(e =>
                {
                    if (e.LocalRoot == this || e == this) return false;
                    return keyCollection.IsKeyInGroup(errorBlock.Block.Key, e.Data.Component.Reference);
                }).ForEach(e =>
                {
                    var next = factory.CreateArg(e.Data.Component, e.LocalRoot)
                        .Where(c => !c.SetBlock(errorBlock.Block, keyCollection, factory)
                            .HasFlag(ParseStatus.Error)
                        ).ToArray();
                    e.Parent.AddRange(next);
                });
            }

            return Status;
        }

        private IBlock LastBlock(IBlock block)
        {
            if (block is ConstructBlock)
            {
                return (block as ConstructBlock).Content.Count() == 0
                    ? block
                    : LastBlock((block as ConstructBlock).Content.Last());
            }
            return block;
        }

        public event ConstructCreated ConstructCreated;

        public ParseStatus CompleteFrom(IParseArgs argEnd, KeyCollection keyCollection, IParseArgFactory factory)
        {
            if (argEnd.LocalRoot != this) throw new Exception("The argEnd should have this as it's localroot");
            if (!argEnd.Status.HasFlag(ParseStatus.CanEnd)) throw new Exception("The argEnd should have the status of CanEnd at least");

            var block = new ConstructBlock(Key, argEnd.LocalPath().Select(arg => arg.Block));
            ParseStatus result = SetBlock(block, keyCollection, factory);
            TriggerCreateConstruct(block);

            // remove children that were used to complete this construct.
            Children.Where(arg => arg.LocalRoot == this).ForEach(arg => Remove(arg));

            return result;
        }

        public ConstructArgs SplitCompleteFrom(IParseArgs argEnd, KeyCollection keyCollection, IParseArgFactory factory)
        {
            var block = new ConstructBlock(Key, argEnd.LocalPath().Select(arg => arg.Block));
            ConstructArgs arg = null;
            if (Parent != null)
            {
                arg = new ConstructArgs(block, Key, Data, LocalRoot, keyCollection, factory);
                Parent.Add(arg);
            }
            TriggerCreateConstruct(block);
            return arg;
        }

        private void TriggerCreateConstruct(ConstructBlock block)
        {
            if (ConstructCreated != null)
            {
                ConstructCreated(block);
            }
        }

        public IEnumerable<IParseArgs> LocalEnds()
        {
            return Children.Where(c => c.LocalRoot == this)
                .SelectMany(c => LocalEnds(c));
        }

        private IEnumerable<IParseArgs> LocalEnds(IParseArgs arg)
        {
            if (arg.Children.Count() == 0)
            {
                return new IParseArgs[] { arg };
            }
            return arg.Children.Where(c => c.LocalRoot == this)
                .SelectMany(c => LocalEnds(c));
        }
    }
}
