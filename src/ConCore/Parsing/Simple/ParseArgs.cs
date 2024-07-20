using BranchList;
using ConCore.Blocks;
using ConCore.Key.Components;
using ConCore.Parsing.Simple.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace ConCore.Parsing.Simple
{
    public class ParseArgs : TypeBranchNode<ParseArgs, IParseArgs>, IParseArgs
    {
        public ParseStatus Status { get; set; }

        public IValueComponentData Data { get; }

        public ILocalRoot? LocalRoot { get; }

        public IBlock? Block { get; protected set; }

        public ParseArgs(IValueComponentData data, ILocalRoot? localRoot, IBlock? block)
            : this(data, localRoot)
        {
            Block = block;
        }
        public ParseArgs(IValueComponentData data, ILocalRoot? localRoot)
            : base()
        {
            Status = ParseStatus.None;
            Data = data;
            LocalRoot = localRoot;
        }

        public virtual IList<IValueComponentData?> GetNextComponents()
        {
            return Data.GetNextComponents();
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
