using CC.Grouping;
using CC.Parcing.ComponentTypes;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Parcing
{
    public class ParseFactory : IParseFactory
    {
        private readonly KeyCollection Keys;
        private readonly IParseArgFactory ArgsFactory;
        private readonly IParseArgs ParseTree;

        public ParseFactory(IConstruct startConstruct, KeyCollection keys, IParseArgFactory argsFactory = null)
        {
            Keys = keys;
            ArgsFactory = argsFactory ?? new ParseArgFactory(keys);
            ParseTree = ArgsFactory.NewArg(startConstruct);
        }

        public List<ValueComponent> GetNextKeys()
        {
            return ParseTree.Ends().Select(arg => arg.Component).ToList();
        }

        public void UseBlock(Block block)
        {
            ParseTree.Ends().ForEach(arg => arg.UseBlock(block, Keys, ArgsFactory));
        }
    }
}
