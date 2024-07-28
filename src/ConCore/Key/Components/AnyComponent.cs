using System.Collections.Generic;
using System.Linq;

namespace ConCore.Key.Components
{
    public class AnyComponent : Component
    {
        public AnyComponent(List<Component> options)
            : base(options) { }

        public override IList<IValueComponentData> GetNextComponents(IComponentData parent)
        {
            return Children.SelectMany(child => child.GetNextComponents(parent)).ToList();
        }
    }
}
