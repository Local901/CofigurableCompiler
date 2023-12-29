using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CC.Key.ComponentTypes
{
    public class RepeatComponentData : ComponentData<RepeatComponent>
    {
        public readonly int Repeats;

        public RepeatComponentData(IComponentData parent, RepeatComponent component)
            : this(parent, component, 0) { }
        public RepeatComponentData(IComponentData parent, RepeatComponent component, int repeats)
            : base(parent, component)
        {
            Repeats = repeats;
        }

        public override IList<IValueComponentData> GetNextComponents()
        {
            var data = new RepeatComponentData(Parent, Component, Repeats + 1);
            var result = new List<IValueComponentData>();

            // Repeate while Maximum is larger than the number of repeates or repeat if Maximum is 0.
            if (data.Repeats < Component.Maximum || Component.Maximum == 0)
            {
                result.AddRange(Component.Children[0].GetNextComponents(data));
            }
            if (data.Repeats >= Component.Minimum)
            {
                result.AddRange(
                    Parent != null
                        ? Parent.GetNextComponents()
                        : IComponent.EMPTY_DATA_LIST
                );
            }
            return result;
        }
    }

    /// <summary>
    /// Repeat child or continue with parent.
    /// </summary>
    public class RepeatComponent : IComponent
    {
        /// <summary>
        /// The minimum number of repeats that should be done before the loop can be left.
        /// </summary>
        public readonly int Minimum;
        /// <summary>
        /// The maximum number of repeats that can be done before beeing forced to leave the repeat.
        /// If maximum is less then 1 the limit will be unlimited.
        /// </summary>
        public readonly int Maximum;

        public RepeatComponent(IComponent child, int minimum = 0, int maximum = 0)
            : base()
        {
            Minimum = minimum;
            Maximum = maximum;

            if (child == null) throw new ArgumentNullException("Child should be a component.");
            if (child is RepeatComponent) throw new ArgumentException("The child of a repeatComponent can not be another repeatComponent");
            Add(child);
        }

        public override IList<IValueComponentData> GetNextComponents(IComponentData parent)
        {
            var data = new RepeatComponentData(parent, this);
            var result = Children[0].GetNextComponents(data);
            if (0 >= Minimum)
            {
                result = result.Concat(
                    parent != null
                        ? parent.GetNextComponents()
                        : EMPTY_DATA_LIST
                ).ToList();
            }
            return result;
        }
    }
}
