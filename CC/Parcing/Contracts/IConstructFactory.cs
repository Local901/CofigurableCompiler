using BranchList;
using CC.Contract;
using CC.Parcing.ComponentTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public interface IConstructFactory
    {
        /// <summary>
        /// The key of the construct that is beeing build.
        /// </summary>
        public IConstruct Key { get; }
        /// <summary>
        /// Indecates the status of the construct factory.
        /// </summary>
        public ConstructFactoryStatus Status { get; }
        /// <summary>
        /// A list with all posible block contents.
        /// </summary>
        public List<List<ComponentArgs>> Contents { get; }

        /// <summary>
        /// This function returns a list of components that it can use next.
        /// </summary>
        /// <returns>A list of component that it will be able to use.</returns>
        public List<ValueComponent> GetWantedComponents();
        /// <summary>
        /// When a block is passed to this function it will try to use it and return all possible ways it can be used.
        /// </summary>
        /// <param name="block">The block to check.</param>
        /// <returns>True if it is posible to use more blocks.</returns>
        public bool TryUseBlock(IBlock block);
        /// <summary>
        /// Make a block from this construct.
        /// </summary>
        /// <returns></returns>
        public IConstructBlock MakeBlock();
        /// <summary>
        /// Make a block from this construct using the content that end at endIndex.
        /// </summary>
        /// <param name="endIndex">end index of last block</param>
        /// <returns></returns>
        public IConstructBlock MakeBlock(int endIndex);
    }
}
