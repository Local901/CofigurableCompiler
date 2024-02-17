using System.Collections.Generic;
using System.Linq;

namespace ConCore.Key.Components
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
