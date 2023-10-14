using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Key.ComponentTypes
{
    public class AnyComponent : IComponent
    {
        public AnyComponent(List<IComponent> options)
            : base(options) { }

        public override IList<IValueComponentData> GetNextComponents(IComponentData parent)
        {
            return Children.SelectMany(child => child.GetNextComponents(parent)).ToList();
        }
    }
}
