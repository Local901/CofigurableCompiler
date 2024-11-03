using ConCore.Blocks;
using ConCore.Key;
using ConCore.Key.Collections;
using ConCore.Lexing;
using ConCore.Parsing.Simple;
using ConCore.Parsing.Simple.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace ConCore.Parsing
{
    public class SimpleParser : IParser
    {
        private ILexer FileLexer;
        private IParseArgFactory ArgsFactory;
        private KeyCollection KeyCollection;

        public SimpleParser(ILexer filelexer, IParseArgFactory argsFactory, KeyCollection keyCollection)
        {
            FileLexer = filelexer;
            ArgsFactory = argsFactory;
            KeyCollection = keyCollection;
        }

        public IBlock? DoParse(KeyLangReference startConstruct)
        {
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
                nextBlocks = new List<IValueBlock>();
                return false;
            }

            nextBlocks = FileLexer.TryNextBlock(keys).Select((result) => result.Block).ToList();
            return nextBlocks != null && nextBlocks.Count != 0;
        }
    }
}
