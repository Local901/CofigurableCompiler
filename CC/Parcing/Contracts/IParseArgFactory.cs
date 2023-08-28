using CC.Blocks;
using CC.Key;
using CC.Key.ComponentTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public interface IParseArgFactory
    {
        /// <summary>
        /// Create the ParseArgs objects for the component using the given block. The created
        /// Args will automaticaly be connected to the parent object. If the a part of the component
        /// path already exists it will continue from the last overlaping component.
        /// </summary>
        /// <param name="componentPath">The path to the component.</param>
        /// <param name="parent">The parent of the first component.</param>
        /// <param name="block">The block content for the last component</param>
        /// <returns></returns>
        public IReadOnlyList<IParseArgs> CreateNextArgs(
            IReadOnlyList<IValueComponentData> componentPath,
            IParseArgs parent,
            IBlock block
        );
    }
}
