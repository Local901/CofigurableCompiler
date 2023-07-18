using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Parcing.ComponentTypes
{
    public class AnyComponent : IComponent
    {
        public AnyComponent(List<IComponent> options)
            : base(options) { }

        public override IList<ValueComponent> GetNextComponents()
        {
            return Children.SelectMany(child => child.GetValueComponents()).ToList();
        }

        public override IList<ValueComponent> GetValueComponents(IComponent startAfter = null)
        {
            if (Parent == null) return new List<ValueComponent> { null };
            return Parent.GetValueComponents(this);
        }
    }
}
