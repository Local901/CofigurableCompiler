using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Key.ComponentTypes
{
    public interface IValueComponentData : IComponentData
    {
        public new ValueComponent Component { get; }
    }
}
