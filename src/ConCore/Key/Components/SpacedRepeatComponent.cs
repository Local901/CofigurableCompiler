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

    public class SpacedRepeatComponent : IComponent
    {
        private readonly IComponent ActualComponent;

        public SpacedRepeatComponent(
            IComponent spacer,
            IComponent content,
            SpacerOptions endWithSpacer = SpacerOptions.OPTIONAL,
            int minimum = 0,
            int maximum = 0
        )
        {
            var list = new List<IComponent>
            {
                content,
                new RepeatComponent(new OrderComponent(new List<IComponent>
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
