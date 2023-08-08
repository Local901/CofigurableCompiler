using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Key
{
    public class KeyGroup : IKey
    {
        public IReadOnlyList<KeyLangReference> Members { get; }

        public KeyGroup()
        {
            Members = new List<KeyLangReference>();
        }
        public KeyGroup(IReadOnlyList<KeyLangReference> members)
        {
            Members = members;
        }

        public override IKey GetKeyFor(object value)
        {
            throw new NotImplementedException();
        }

        public override List<KeyLangReference> GetSubKeyRefs()
        {
            return Members as List<KeyLangReference>;
        }

        public override List<IKey> GetSubKeys()
        {
            return new List<IKey>();
        }
    }
}
