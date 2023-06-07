using CC.Contract;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing
{
    public class Construct : IConstruct
    {
        private List<IComponent> _components;
        public IReadOnlyList<IComponent> Components => _components;


        public Construct()
        {
            _components = new List<IComponent>();
        }


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
