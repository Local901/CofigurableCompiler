using CC.Blocks;
using CC.Key;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing
{
    public delegate void ConstructCreated(ConstructBlock block);

    public interface ILocalRoot : IParseArgs
    {
        /// <summary>
        /// The number of localRoots deep that this arg is located.
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// Complete the LocalRoot with the content gotten from argEnd and remove al chidren that have this as localRoot.
        /// </summary>
        /// <param name="argEnd">An endpoint of the arg branch with a localRoot equal to this object.</param>
        /// <param name="keyCollection"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public ParseStatus CompleteFrom(IParseArgs argEnd, KeyCollection keyCollection, IParseArgFactory factory);
        /// <summary>
        /// Create a completed ParseArg in the parent based on this object.
        /// </summary>
        /// <param name="argEnd">An endpoint of the arg branch with a localRoot equal to this object.</param>
        /// <param name="keyCollection"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public ConstructArgs SplitCompleteFrom(IParseArgs argEnd, KeyCollection keyCollection, IParseArgFactory factory);
        /// <summary>
        /// Get all ends for this localRoot.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IParseArgs> LocalEnds();

        public event ConstructCreated ConstructCreated;
    }
}
