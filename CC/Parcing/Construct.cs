using CC.Contract;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing
{
    public class Construct : IConstruct
    {
        public Construct(IComponent components)
            : base(components)
        { }


        public override IKey GetKey(object value)
        {
            return this;
        }

        public override List<IKey> GetKeys()
        {
            return new List<IKey>();
        }
    }
}
