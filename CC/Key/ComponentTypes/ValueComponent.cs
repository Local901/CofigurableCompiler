using BranchList;
using CC.Key;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Key.ComponentTypes
{
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
