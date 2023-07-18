using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.ComponentTypes
{
    public class OrderComponent : IComponent
    {
        public OrderComponent(List<IComponent> children)
            : base(children)
        {
            if (children.Count == 0) throw new Exception("The list of children should contain components.");
        }

        public override IList<ValueComponent> GetNextComponents()
        {
            return Children[0].GetValueComponents();
        }

        public override IList<ValueComponent> GetValueComponents(IComponent startAfter = null)
        {
            int index = Children.IndexOf(startAfter) + 1;
            if (index == Count || index == 0)
            {
                return Parent == null
                    ? new List<ValueComponent> { null }
                    : Parent.GetValueComponents(this);
            }
            return Children[index].GetValueComponents();
        }
    }
}
