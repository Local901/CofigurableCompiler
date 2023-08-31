﻿using CC.Key.ComponentTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Key
{
    public class Construct : IConstruct
    {
        public Construct(string key, IComponent component)
            : base(key, component)
        { }


        public override IKey GetKeyFor(object value)
        {
            throw new NotImplementedException();
        }

        public override List<IKey> GetSubKeys()
        {
            return new List<IKey>();
        }

        public override List<KeyLangReference> GetSubKeyRefs()
        {
            return new List<KeyLangReference>();
        }
    }
}
