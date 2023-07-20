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

        public override IKey GetKeyFor(string value)
        {
            throw new NotImplementedException();
        }

        public override List<IKey> GetKeys()
        {
            return Members.SelectMany(m => m.GetKeys()).Distinct().ToList();
        }
    }
}
