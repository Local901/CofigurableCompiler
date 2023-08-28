using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Key.ComponentTypes
{
    public class OrderComponentData : ComponentData<OrderComponent>
    {
        public readonly int Index;

        public OrderComponentData(IComponentData parent, OrderComponent component)
            : this(parent, component, 0) { }
        public OrderComponentData(IComponentData parent, OrderComponent component, int index)
            : base(parent, component)
        {
            Index = index;
        }

        public override IList<IValueComponentData> GetNextComponents()
        {
            if (Index + 1 == Component.Children.Count)
            {
                return Parent != null
                    ? Parent.GetNextComponents()
                    : IComponent.EMPTY_DATA_LIST;
            }

            var data = new OrderComponentData(Parent, Component, Index + 1);
            return Component.Children[data.Index].GetNextComponents(data);
        }
    }

    public class OrderComponent : IComponent
    {
        public OrderComponent(List<IComponent> children)
            : base(children)
        {
            if (children.Count == 0) throw new Exception("The list of children should contain components.");
        }

        public override IList<IValueComponentData> GetNextComponents(IComponentData parent)
        {
            var data = new OrderComponentData(parent, this);
            return Children[0].GetNextComponents(data);
        }
    }
}
