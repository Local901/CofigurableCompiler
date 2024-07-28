using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Key.Components
{
    public enum SpacerOptions
    {
        OPTIONAL,
        NEVER,
        ALLWAYS
    }

    public class SpacedRepeatComponent : Component
    {
        private readonly Component ActualComponent;

        public SpacedRepeatComponent(
            Component spacer,
            Component content,
            SpacerOptions endWithSpacer = SpacerOptions.OPTIONAL,
            int minimum = 0,
            int maximum = 0
        )
        {
            var list = new List<Component>
            {
                content,
                new RepeatComponent(new OrderComponent(new List<Component>
                {
                    spacer,
                    content,
                }), Math.Max(0, minimum - 1), Math.Min(0, maximum - 1)),
            };

            // Add a spacer at the end following the option
            if (endWithSpacer != SpacerOptions.NEVER)
            {
                if (endWithSpacer == SpacerOptions.ALLWAYS)
                {
                    list.Add(spacer);
                }
                else
                {
                    list.Add(new RepeatComponent(spacer, 0, 1));
                }
            }

            ActualComponent = new OrderComponent(list);
        }

        public override IList<IValueComponentData?> GetNextComponents(IComponentData? parent)
        {
            return ActualComponent.GetNextComponents(parent);
        }
    }
}
