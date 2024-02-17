using ConCore.Blocks;
using ConCore.Key;
using ConCore.Key.Components;
using System.Collections.Generic;

namespace ConCore.Parsing.Simple.Contracts
{
    public interface IParseArgFactory
    {
        /// <summary>
        /// Create the ParseArgs object for the component using the given block. The created
        /// Args will automaticaly be connected to the parent object. If the a part of the component
        /// path already exists it will continue from the last overlaping component.
        /// </summary>
        /// <param name="componentPath">The path to the component.</param>
        /// <param name="parent">The parent of the first component.</param>
        /// <param name="block">The block content for the last component</param>
        /// <returns></returns>
        public IParseArgs CreateNextArgs(
            IReadOnlyList<IValueComponentData> componentPath,
            IParseArgs parent,
            IBlock block
        );

        /// <summary>
        /// Create the root for parsing which has the Data property set so the nextComponents function returns
        /// IValueComponentdata for the provided key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ILocalRoot CreateRoot(KeyLangReference key);

        public ArgsData GenerateNextArgsData(IParseArgs arg);
    }
}
