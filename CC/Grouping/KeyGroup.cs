using CC.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Grouping
{
    public class KeyGroup : IKey
    {
        public List<KeyGroup> Members { get; }
        public bool IsTemp { get; set; }

        public KeyGroup()
        {
            Members = new List<KeyGroup>();
            IsGroup = true;
        }

        public override IKey GetKey(object value)
        {
            return this;
        }

        public override List<IKey> GetKeys()
        {
            return Members.Cast<IKey>().ToList();
        }
    }
}
