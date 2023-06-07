using BranchList;
using CC.Contract;
using CC.Grouping;
using CC.Lexing;
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
        private IBlock ResultBlock;
        private BranchNode<IConstructParsingArgs> BranchList;
        
        public FileParcer(FileLexer filelexer, KeyCollection keyCollection)
        {
            FileLexer = filelexer;
            KeyCollection = keyCollection;
        }

        public void DoParse(out IBlock block, IConstruct startConstruct)
        {
            ResultBlock = new Block { Key = startConstruct };

            // start branch
            BranchList = new BranchNode<IConstructParsingArgs>(new ConstructParsingArgs(startConstruct));

            IBlock nextBlock;
            while( TryGetNextBlock(out nextBlock) )
            {
                UpdateBranches(BranchList);
                UseBlock(BranchList, nextBlock);
            }

            block = ResultBlock;
        }

        public bool TryGetNextBlock(out IBlock nextBlock)
        {
            return FileLexer.TryNextBlock(out nextBlock);
        }

        private void UpdateBranches(BranchList<IConstructParsingArgs> branch)
        {
            // If current ends have an expectation for Constructs
            // add a child branch.
            throw new NotImplementedException();
        }

        private void UseBlock(BranchList<IConstructParsingArgs> branch, IBlock nextBlock)
        {
            // Try use block on ends of branch.

            // When construct arg is complete make block and
            // add to parent.
            throw new NotImplementedException();
        }
    }
}
