using CC.Blocks;
using CC.Key;
using CC.Key.ComponentTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parsing.Contracts
{
    public interface IParseArgs
    {
        /// <summary>
        /// Indecate the status of this parse step. It is None when no block has been used for this step.
        /// </summary>
        public ParseStatus Status { get; set; }
        /// <summary>
        /// The component related to this step in parsing.
        /// </summary>
        public IValueComponentData Data { get; }
        /// <summary>
        /// The reference to the root object that this step originates from. LocalRoot can be null to indecate the root of the entire tree.
        /// </summary>
        public ILocalRoot LocalRoot { get; }
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

        public IList<IValueComponentData> GetNextComponents();

        public List<IParseArgs> Ends();
        /// <summary>
        /// Returns the path from the localRoot until this, without the LocalRoot.
        /// </summary>
        /// <returns></returns>
        public List<IParseArgs> LocalPath();
        public void Add(IParseArgs arg);
        public void AddRange(IEnumerable<IParseArgs> args);
        public bool Remove(IParseArgs arg);
        /// <summary>
        /// Remove until a node has more than one child.
        /// </summary>
        public void RemoveBranch();
    }
}
