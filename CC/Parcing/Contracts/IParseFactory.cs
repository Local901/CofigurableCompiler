﻿using CC.Blocks;
using CC.Key;
using CC.Key.ComponentTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public interface IParseFactory
    {
        /// <summary>
        /// Get the list of keys that are expected next.
        /// </summary>
        /// <returns></returns>
        public List<KeyLangReference> GetNextKeys();
        /// <summary>
        /// Use a block to progress the parsing.
        /// </summary>
        /// <param name="block"></param>
        public void UseBlock(IBlock block);
        /// <summary>
        /// Get the ArgsData for the provided arg. This contains the arg itself and all the paths to following valueComponents.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public ArgsData GetNextDataOfArgs(IParseArgs arg);
        /// <summary>
        /// Get the number of rounds ran.
        /// </summary>
        public int NumberOfRounds { get; }
        /// <summary>
        /// Get a list of completed ends.
        /// </summary>
        public IReadOnlyList<IParseCompletion> Completed { get; }
        /// <summary>
        /// Complete a endings that have not ended yet.
        /// </summary>
        public void CompleteEnds();
    }
}
