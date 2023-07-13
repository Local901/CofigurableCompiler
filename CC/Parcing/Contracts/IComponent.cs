using BranchList;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public interface IComponent : IBranchNode<IComponent>
    {
        /// <summary>
        /// Return a list of components that are expected after the current one.
        /// A element in the list of components can be `null` when a posible end point was found.
        /// </summary>
        /// <returns>A list of components that can follow after this component or a element in the list is null when a endpoint is encounterd.</returns>
        public IList<IComponent> GetNextComponents();
        /// <summary>
        /// Return a list of components that are expected after the current one.
        /// The property startFrom is null when called for the first time and should
        /// be equal to a child object when called from that child object.
        /// A element in the list of components can be `null` when a posible end point was found.
        /// </summary>
        /// <param name="startFrom">Default null, any child when the search should start from that child.</param>
        /// <returns>A list of components that can follow after this component or a element in the list is null when a endpoint is encounterd.</returns>
        public IList<IComponent> GetNextComponents(IComponent startFrom = null);
        /// <summary>
        /// Return a list of components that are expected after the current one.
        /// The property startFrom is null when called for the first time and should
        /// be equal to a child object when called from that child object.
        /// A element in the list of components can be `null` when a posible end point was found.
        /// </summary>
        /// <param name="startFrom">Default null, any child when the search should start from that child.</param>
        /// <returns>A list of components that can follow after this component or a element in the list is null when a endpoint is encounterd.</returns>
        protected IList<IComponent> GetNextComponents(IComponent startFrom = null, bool isCalledFirst = true);
    }
}
