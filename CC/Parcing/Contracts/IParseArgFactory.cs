using CC.Contract;
using CC.Grouping;
using CC.Parcing.ComponentTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public interface IParseArgFactory
    {
        /// <summary>
        /// Create a new ParseArg of the correct type based on the component.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="localRoot"></param>
        /// <returns></returns>
        public IEnumerable<IParseArgs> CreateArg(ValueComponent component, ILocalRoot localRoot);
        /// <summary>
        /// Create a new ParseArg of the correct type based on the construct key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IParseArgs CreateArg(IConstruct key);
    }
}
