using BranchList;
using CC.Contract;
using CC.Grouping;
using CC.Parcing.ComponentTypes;
using CC.Parcing.Contracts;

namespace CC.Parcing
{
    public abstract class ParseArgs : TypeBranchNode<ParseArgs, IParseArgs>, IParseArgs
    {
        public ValueComponent Component { get; }

        public IParseArgs LocalRoot { get; }

        public IBlock Block { get; protected set; }

        public ParseArgs(ValueComponent component, IParseArgs localRoot)
            : base()
        {
            Component = component;
            LocalRoot = localRoot;
        }

        public abstract ParseStatus UseBlock(IBlock block, KeyCollection keyCollection, IParseArgFactory factory);
    }
}
