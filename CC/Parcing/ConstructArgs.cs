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

        public ParseStatus CompleteFrom(IParseArgs argEnd, KeyCollection keyCollection, IParseArgFactory factory)
        {
            if (argEnd.LocalRoot != this) throw new Exception("The argEnd should have this as it's localroot");
            if (!argEnd.Status.HasFlag(ParseStatus.CanEnd)) throw new Exception("The argEnd should have the status of CanEnd at least");

            ParseStatus result = UseBlock(new ConstructBlock(Key, argEnd.LocalPath().Select(arg => arg.Block)), keyCollection, factory);

            // remove children that were used to complete this construct.
            Children.Where(arg => arg.LocalRoot == this).ForEach(arg => Remove(arg));

            return result;
        }

        public ParseStatus SplitCompleteFrom(IParseArgs argEnd, KeyCollection keyCollection, IParseArgFactory factory)
        {
            throw new NotImplementedException();
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
