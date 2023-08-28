using BranchList;
using CC.Blocks;
using CC.Key;
using CC.Key.ComponentTypes;
using CC.Parcing;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC
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

        public void DoParse(out ConstructBlock block, IConstruct startConstruct)
        {
            FileLexer.Reset();
            IReadOnlyList<IValueComponentData> Ends = null;

            IParseFactory factory = new ParseFactory(startConstruct, KeyCollection);

            IBlock nextBlock;
            while (TryGetNextBlock(out nextBlock, factory))
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
            var refs = factory.GetNextKeys()
                .Distinct()
                .SelectMany(l => KeyCollection.GetLanguage(l.Lang).GetAllKeys())
                .Select(k => k.Reference);
            return FileLexer.TryNextBlock(out nextBlock, refs);
        }

        private IReadOnlyList<IValueComponentData> UpdateEnds(IReadOnlyList<IValueComponentData> Ends, IBlock block, IConstruct startConstruct)
        {
            IReadOnlyList<IValueComponentData> nextEnds = Ends == null
                ? nextEnds = startConstruct.Component.GetNextComponents(null).ToList()
                : nextEnds = Ends.SelectMany(e => e.GetNextComponents()).ToList();

            return nextEnds;
        }
    }
}
