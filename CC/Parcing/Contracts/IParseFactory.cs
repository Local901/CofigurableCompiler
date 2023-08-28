using CC.Blocks;
using CC.Key;
using CC.Key.ComponentTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public interface IParseFactory
    {
        public ConstructBlock LastCompletion { get; }
        public List<KeyLangReference> GetNextKeys();
        public void UseBlock(IBlock block);
        /// <summary>
        /// Get the ArgsData for the provided arg. This contains the arg itself and all the paths to following valueComponents.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public ArgsData GetNextDataOfArgs(IParseArgs arg);
    }
}
