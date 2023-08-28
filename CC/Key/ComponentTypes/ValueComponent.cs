using BranchList;
using CC.Key;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Key.ComponentTypes
{
    public class ValueComponentData : ComponentData<ValueComponent>, IValueComponentData
    {
        public ValueComponentData(IComponentData parent, ValueComponent component)
            : base(parent, component) { }

        public override IList<IValueComponentData> GetNextComponents()
        {
            if (Parent == null) return IComponent.EMPTY_DATA_LIST;
            return Parent.GetNextComponents();
        }
    }

    public class ValueComponent : IComponent
    {
        public KeyLangReference Reference { get; }
        public string Name { get; }

        public ValueComponent(KeyLangReference key) 
            : this(key, null) { }
        public ValueComponent(KeyLangReference key, string name)
        {
            Reference = key;
            Name = name;
        }

        public override IList<IValueComponentData> GetNextComponents(IComponentData parent)
        {
            return new List<IValueComponentData> { new ValueComponentData(parent, this) };
        }
    }
}
