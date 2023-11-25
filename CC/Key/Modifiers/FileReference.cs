using CC.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Key.Modifiers
{
    public enum FileReferenceType
    {
        Any,
        Absolute,
        Relative,
    }

    public interface FileReferenceArgs
    {
        KeyLangReference KeyReference { get; set; }
        string ValuePath { get; set; }
        FileReferenceType ReferenceType { get; set; }
    }

    public class FileReference : IParseTreeFilter
    {
        public readonly FileReferenceArgs Args;

        public FileReference(IBlockReader blockReader, FileReferenceArgs args)
            :base(blockReader)
        {
            Args = args;
        }

        public override IBlock[] FindBlocks(IBlock block)
        {
            return BlockReader.SelectAll(block, (c) => c.Key.Reference == Args.KeyReference);
        }
    }
}
