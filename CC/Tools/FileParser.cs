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

        public void DoParse(out IBlock block, KeyLangReference startConstruct)
        {
            FileLexer.Reset();

            IParseFactory factory = new ParseFactory(startConstruct, KeyCollection, ArgsFactory);

            IBlock nextBlock;
            while (TryGetNextBlock(out nextBlock, factory))
            {
                factory.UseBlocks(new List<IBlock> { nextBlock });
            }

            var ends = factory.Completed.Where(c => c.Round == factory.NumberOfRounds).ToList();
            if (ends.Count() == 0)
            {
                factory.CompleteEnds();
                ends = factory.Completed.Where(c => c.Round == factory.NumberOfRounds).ToList();
            }

            // TODO: look for the errors.
            block = ends.FirstOrDefault()?.Block;
        }

        /// <summary>
        /// Get the next block from the FileLexer.
        /// </summary>
        /// <param name="nextBlock"></param>
        /// <returns>True if the block has been created.</returns>
        private bool TryGetNextBlock(out IBlock nextBlock, IParseFactory factory)
        {
            var keys = factory.GetNextKeys();
            if (keys.Count == 0)
            {
                nextBlock = null;
                return false;
            }
            var refs = keys
                .Select(k => k.Lang)
                .Distinct()
                .SelectMany(l => KeyCollection.GetLanguage(l).GetAllKeys())
                .Select(k => k.Reference);
            return FileLexer.TryNextBlock(out nextBlock, refs);
        }
    }
}
