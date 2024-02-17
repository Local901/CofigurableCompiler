using BranchList;
using System.Collections.Generic;

namespace ConCore.Key.Components
{
    public abstract class IComponent : BranchNode<IComponent>
    {
        public static readonly List<IValueComponentData> EMPTY_DATA_LIST = new List<IValueComponentData> { null };
        public IComponent()
            : base() { }
        public IComponent(List<IComponent> children)
            : base(children) { }

        /// <summary>
        /// Return a list of components that are expected after the current one.
        /// A element in the list of components can be `null` when a posible end point was found.
        /// </summary>
        /// <returns>A list of components that can follow after this component or a element in the list is null when a endpoint is encounterd.</returns>
        public abstract IList<IValueComponentData> GetNextComponents(IComponentData parent);
    }
}
