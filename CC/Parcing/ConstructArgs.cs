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
    public class ConstructArgs : ParseArgs
    {
        public IConstruct Key { get; }

        public ConstructArgs(IConstruct key, ValueComponent component, IParseArgs localRoot, IParseArgFactory factory)
            : base(component, localRoot)
        {
            Key = key;
            AddRange(
                key.Components
                    .GetNextComponents()
                    .Select(comp => factory.NewArg(comp, this))
            );
        }

        public override ParseStatus UseBlock(IBlock block, KeyCollection keyCollection, IParseArgFactory factory)
        {
            throw new NotImplementedException();
        }
    }
}
