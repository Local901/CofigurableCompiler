using BranchList;
using CC.Contract;
using CC.Grouping;
using CC.Parcing.ComponentTypes;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Parcing
{
    public class ConstructArgs : ParseArgs, ILocalRoot
    {
        public IConstruct Key { get; }

        private ConstructArgs(IBlock block, IConstruct key, ValueComponent component, ILocalRoot localRoot, KeyCollection keyCollection, IParseArgFactory factory)
            : base(component, localRoot)
        {
            Key = key;
            UseBlock(block, keyCollection, factory);
        }
        public ConstructArgs(IConstruct key, ValueComponent component, ILocalRoot localRoot, IParseArgFactory factory)
            : base(component, localRoot)
        {
            Key = key;
            AddRange(
                key.Components
                    .GetNextComponents()
                    .SelectMany(comp => factory.CreateArg(comp, this))
            );
        }

        public event ConstructCreated ConstructCreated;

        public ParseStatus CompleteFrom(IParseArgs argEnd, KeyCollection keyCollection, IParseArgFactory factory)
        {
            if (argEnd.LocalRoot != this) throw new Exception("The argEnd should have this as it's localroot");
            if (!argEnd.Status.HasFlag(ParseStatus.CanEnd)) throw new Exception("The argEnd should have the status of CanEnd at least");

            var block = new ConstructBlock(Key, argEnd.LocalPath().Select(arg => arg.Block));
            ParseStatus result = UseBlock(block, keyCollection, factory);
            TriggerCreateConstruct(block);

            // remove children that were used to complete this construct.
            Children.Where(arg => arg.LocalRoot == this).ForEach(arg => Remove(arg));

            return result;
        }

        public ParseStatus SplitCompleteFrom(IParseArgs argEnd, KeyCollection keyCollection, IParseArgFactory factory)
        {
            var block = new ConstructBlock(Key, argEnd.LocalPath().Select(arg => arg.Block));
            ParseStatus result = ParseStatus.None;
            if (Parent != null)
            {
                var arg = new ConstructArgs(block, Key, Component, LocalRoot, keyCollection, factory);
                Parent.Add(arg);
                result = arg.Status;
            }
            TriggerCreateConstruct(block);
            return result;
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
            if (Children.Count() == 0)
            {
                return new IParseArgs[] { arg };
            }
            return Children.Where(c => c.LocalRoot == this)
                .SelectMany(c => LocalEnds(c));
        }
    }
}
