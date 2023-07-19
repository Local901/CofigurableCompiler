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
        private IBlock ResultBlock;
        private ValueBranchNode<IConstructFactory> BranchList;
        
        public FileParcer(FileLexer filelexer, KeyCollection keyCollection)
        {
            FileLexer = filelexer;
            KeyCollection = keyCollection;
        }

        public void DoParse(out IBlock block, IConstruct startConstruct)
        {
            ResultBlock = new Block { Key = startConstruct };

            // start branch
            BranchList = new ValueBranchNode<IConstructFactory>(new ConstructFactory(startConstruct, KeyCollection));

            IBlock nextBlock;
            while( TryGetNextBlock(out nextBlock) )
            {
                AddSubConstructs(BranchList);
                UseBlock(BranchList, nextBlock);
            }

            block = ResultBlock;
        }

        /// <summary>
        /// Get the next block from the FileLexer.
        /// </summary>
        /// <param name="nextBlock"></param>
        /// <returns>True if the block has been created.</returns>
        private bool TryGetNextBlock(out IBlock nextBlock)
        {
            return FileLexer.TryNextBlock(out nextBlock);
        }

        private void AddSubConstructs(ValueBranchNode<IConstructFactory> tree)
        {
            tree.Ends()
                .Where((arg) => !arg.Value.IsComplete && arg != tree)
                .ForEach((arg) =>
                {
                    List<ValueComponent> components = arg.Value.GetWantedComponents();
                    components.SelectMany((comp) => KeyCollection.GetRelationOfKey(comp.Key).Keys)
                        .Distinct()
                        .Where((key) => key is IConstruct)
                        .Cast<IConstruct>()
                        .ForEach((construct) => arg.Add(new ConstructFactory(construct, KeyCollection)));
                    AddSubConstructs(arg);
                });
        }

        private void UseBlock(ValueBranchNode<IConstructFactory> branch, IBlock nextBlock)
        {
            // Try use block on ends of branch.

            // When construct arg is complete make block and
            // add to parent.
            throw new NotImplementedException();
        }
    }
}
