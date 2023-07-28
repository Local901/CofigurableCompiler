using CC.Contract;
using CC.Parcing.ComponentTypes;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing
{
    public class Construct : IConstruct
    {
        public Construct(string name, IComponent components)
            : base(name, components)
        { }


        public override IKey GetKeyFor(string value)
        {
            throw new NotImplementedException();
        }

        public override List<IKey> GetKeys()
        {
            return new List<IKey> { this };
        }
    }
}
