using CC.Contract;
using CC.Grouping;
using CC.Parcing.ComponentTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public interface IParseArgs
    {
        /// <summary>
        /// The component related to this step in parsing.
        /// </summary>
        public ValueComponent Component { get; }
        /// <summary>
        /// The reference to the root object that this step originates from. LocalRoot can be null to indecate the root of the entire tree.
        /// </summary>
        public IParseArgs LocalRoot { get; }
        /// <summary>
        /// The block that has been used for this step.
        /// </summary>
        public IBlock Block { get; }
        /// <summary>
        /// Parent is the parse step that came before this one.
        /// </summary>
        public IParseArgs Parent { get; }
        /// <summary>
        /// Children are the parse steps that came after this step. The children only apear after this step has been completed.
        /// </summary>
        public List<IParseArgs> Children { get; }

        /// <summary>
        /// This function will use the block provided to it.
        /// </summary>
        /// <param name="block"></param>
        public ParseStatus UseBlock(IBlock block, KeyCollection keyCollection, IParseArgFactory factory);

        public List<IParseArgs> Ends();
        public List<IParseArgs> Path();
        public void Add(IParseArgs arg);
        public void AddRange(IEnumerable<IParseArgs> args);
    }
}
