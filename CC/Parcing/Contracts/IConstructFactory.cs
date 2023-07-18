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
        /// Indecates if the construct is complete.
        /// </summary>
        public bool IsComplete { get; }
        /// <summary>
        /// Indicates if more blocks can be provided to expand the construct.
        /// </summary>
        public bool CanContinue { get; }

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
        public IBlock MakeBlock();
    }
}
