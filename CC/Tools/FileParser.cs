using BranchList;
using CC.Blocks;
using CC.Key;
using CC.Key.ComponentTypes;
using CC.Parsing;
using CC.Parsing.Contracts;
using CC.Tools.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Tools
{
    public class FileParser : IParser
    {
        private ILexer FileLexer;
        private IParseArgFactory ArgsFactory;
        private KeyCollection KeyCollection;

        public FileParser(ILexer filelexer, IParseArgFactory argsFactory, KeyCollection keyCollection)
        {
            FileLexer = filelexer;
            ArgsFactory = argsFactory;
            KeyCollection = keyCollection;
        }

        public IBlock DoParse(KeyLangReference startConstruct)
        {
            FileLexer.Reset();

            IParseFactory factory = new ParseFactory(startConstruct, KeyCollection, ArgsFactory);

            IList<IValueBlock> nextBlocks;
            while (TryGetNextBlock(out nextBlocks, factory))
            {
                factory.UseBlocks(nextBlocks);
            }

            var ends = factory.Completed.Where(c => c.Round == factory.NumberOfRounds).ToList();
            if (ends.Count() == 0)
            {
                factory.CompleteEnds();
                ends = factory.Completed.Where(c => c.Round == factory.NumberOfRounds).ToList();
            }

            // TODO: look for the errors.
            return ends.FirstOrDefault()?.Block;
        }

        /// <summary>
        /// Get the next block from the FileLexer.
        /// </summary>
        /// <param name="nextBlocks"></param>
        /// <returns>True if the block has been created.</returns>
        private bool TryGetNextBlock(out IList<IValueBlock> nextBlocks, IParseFactory factory)
        {
            var keys = factory.GetNextKeys();
            if (keys.Count == 0)
            {
                nextBlocks = null;
                return false;
            }

            nextBlocks = FileLexer.TryNextBlock(keys);
            return nextBlocks != null && nextBlocks.Count != 0;
        }
    }
}
