using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Key.ComponentTypes
{
    public abstract class ComponentData<T> : IComponentData where T : IComponent
    {
        public T Component { get; }
        IComponent IComponentData.Component => Component;

        public IComponentData Parent { get; }

        public ComponentData(IComponentData parent, T component)
        {
            Component = component;
            Parent = parent;
        }

        /// <summary>
        /// Get all next value component data.
        /// </summary>
        /// <returns></returns>
        public abstract IList<IValueComponentData> GetNextComponents();
    }
}
