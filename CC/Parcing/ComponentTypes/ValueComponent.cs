using BranchList;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.ComponentTypes
{
    public class ValueComponent : IComponent
    {
        public string Key { get; }
        public string Name { get; }

        public ValueComponent(string key) 
            : this(key, null) { }
        public ValueComponent(string key, string name)
        {
            Key = key;
            Name = name;
        }

        public override IList<ValueComponent> GetNextComponents()
        {
            if (Parent == null) return new List<ValueComponent> { null };
            return Parent.GetValueComponents(this);
        }

        public override IList<ValueComponent> GetValueComponents(IComponent startAfter = null)
        {
            return new List<ValueComponent> { this };
        }
    }
}
