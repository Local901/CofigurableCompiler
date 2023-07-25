using CC.Contract;
using CC.Grouping;
using CC.Parcing.ComponentTypes;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CC.Parcing
{
    public class ComponentArgs : ParseArgs
    {
        public ComponentArgs(ValueComponent component, IParseArgs localRoot)
            : base(component, localRoot)
        { }

        public override ParseStatus UseBlock(IBlock block, KeyCollection keyCollection, IParseArgFactory factory)
        {
            ParseStatus status = ParseStatus.None;

            if (!keyCollection.IsKeyOfGroup(block.Key, Component.Key))
            {
                // make error block
                status |= ParseStatus.Error;
                block = new IssueBlock
                {
                    Type = IssueType.Error,
                    Key = keyCollection.GetKey(Component.Key),
                    Index = Parent == null ? 0 : Parent.Block.EndIndex,
                    EndIndex = block.Index
                };
            }
            Block = block.Copy(Component.Name);

            var components = Component.GetNextComponents();
            if (components.Any(comp => comp == null))
            {
                status |= ParseStatus.CanEnd;
            }

            components = components.Where(comp => comp != null).ToList();
            if (components.Count() == 0)
            {
                status |= ParseStatus.ReachedEnd;
            }
            AddRange(components.Select(comp => factory.NewArg(comp, LocalRoot)));

            return status;
        }
    }
}
