using CC.Key;
using CC.Key.ComponentTypes;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<IParseArgs> CreateArg(IReadOnlyList<IValueComponentData> data, ILocalRoot localRoot)
        {
            var keys = Keys.GetAllProminentSubKeys(data.Component.Reference, true);
            var constructsKeys = keys.OfType<IConstruct>();

            if (keys.Count() != constructsKeys.Count()) yield return new ParseArgs(data, localRoot);

            foreach(var construct in constructsKeys)
            {
                yield return new ConstructArgs(construct, data, localRoot, this);
            }
        }

        public ILocalRoot CreateArg(IConstruct key)
        {
            return new ConstructArgs(key, new ValueComponent(key.Reference, ""), null, this);
        }
    }
}
