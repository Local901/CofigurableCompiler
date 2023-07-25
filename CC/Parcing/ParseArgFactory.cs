using CC.Grouping;
using CC.Parcing.ComponentTypes;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing
{
    public class ParseArgFactory : IParseArgFactory
    {
        private readonly KeyCollection Keys;

        public ParseArgFactory(KeyCollection keys)
        {
            Keys = keys;
        }

        public IParseArgs NewArg(ValueComponent component, IParseArgs localRoot)
        {
            throw new NotImplementedException();
        }

        public IParseArgs NewArg(IConstruct key)
        {
            return new ConstructArgs(key, new ValueComponent(key.Key, ""), null);
        }
    }
}
