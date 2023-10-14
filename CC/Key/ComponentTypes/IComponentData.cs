using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Key.ComponentTypes
{
    /// <summary>
    /// Component Data is a object that hold the data for a component. The data object should never change only recreated with diferent values.
    /// </summary>
    public interface IComponentData
    {
        public IComponentData Parent { get; }
        public IComponent Component { get; }

        public IList<IValueComponentData> GetNextComponents();
    }
}
