using BranchList;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Key.ComponentTypes
{
    public abstract class IComponent : BranchNode<IComponent>
    {
        public IComponent()
            : base() { }
        public IComponent(List<IComponent> children)
            : base(children) { }

        /// <summary>
        /// Return a list of components that are expected after the current one.
        /// A element in the list of components can be `null` when a posible end point was found.
        /// </summary>
        /// <returns>A list of components that can follow after this component or a element in the list is null when a endpoint is encounterd.</returns>
        public abstract IList<ValueComponent> GetNextComponents();
        /// <summary>
        /// Get a list of ValueComponents that are found can be placed after the passed value.
        /// A element in the list of components can be `null` when a posible end point was found.
        /// </summary>
        /// <param name="startFrom">The component that the search should start after.</param>
        /// <returns></returns>
        public abstract IList<ValueComponent> GetValueComponents(IComponent startAfter = null);
    }
}
