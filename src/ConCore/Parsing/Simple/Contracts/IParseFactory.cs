using ConCore.Blocks;
using ConCore.Key;
using System.Collections.Generic;

namespace ConCore.Parsing.Simple.Contracts
{
    public interface IParseFactory
    {
        /// <summary>
        /// Get the list of keys that are expected next.
        /// </summary>
        /// <returns></returns>
        public List<KeyLangReference> GetNextKeys();
        /// <summary>
        /// Use blocks to progress the parsing.
        /// </summary>
        /// <param name="blocks"></param>
        public void UseBlocks(IEnumerable<IBlock> blocks);
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
