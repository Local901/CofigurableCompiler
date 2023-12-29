using CC.Blocks;
using CC.Key;
using CC.Parsing.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parsing
{
    public delegate void ConstructCreated(ConstructBlock block);

    public interface ILocalRoot : IParseArgs
    {
        /// <summary>
        /// The number of localRoots deep that this arg is located.
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// Create a block using the provided branch ending.
        /// </summary>
        /// <param name="localArgEnd"></param>
        /// <returns></returns>
        public IRelationBlock CreateBlock(IParseArgs localArgEnd);
    }
}
