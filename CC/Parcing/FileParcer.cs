using BranchList;
using CC.Contract;
using CC.Grouping;
using CC.Lexing;
using CC.Parcing.ComponentTypes;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Parcing
{
    public class FileParcer : IFileParcer
    {
        private FileLexer FileLexer;
        private KeyCollection KeyCollection;
        
        public FileParcer(FileLexer filelexer, KeyCollection keyCollection)
        {
            FileLexer = filelexer;
            KeyCollection = keyCollection;
        }

        public void DoParse(out IBlock block, IConstruct startConstruct)
        {
            IParseFactory factory = new ParseFactory(startConstruct, KeyCollection);

            IBlock nextBlock;
            while( TryGetNextBlock(out nextBlock, factory) )
            {
                factory.UseBlock(nextBlock);
            }

            block = factory.LastCompletion;
        }

        /// <summary>
        /// Get the next block from the FileLexer.
        /// </summary>
        /// <param name="nextBlock"></param>
        /// <returns>True if the block has been created.</returns>
        private bool TryGetNextBlock(out IBlock nextBlock, IParseFactory factory)
        {
            return FileLexer.TryNextBlock(out nextBlock);
        }
    }
}
