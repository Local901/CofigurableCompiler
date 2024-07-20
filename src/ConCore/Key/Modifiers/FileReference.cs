using ConCore.Blocks;
using ConCore.Blocks.Helpers;
using System.Linq;

namespace ConCore.Key.Modifiers
{
    public enum FileReferenceType
    {
        Any,
        Absolute,
        Relative,
    }

    public struct FileReferenceArgs
    {
        public KeyLangReference KeyReference { get; set; }
        public string[] ValuePath { get; set; }
        public FileReferenceType ReferenceType { get; set; }
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

        public virtual string[] FindFileReferences(IBlock block)
        {
            return FindBlocks(block).Select((b) => {
                var result = BlockReader.TraverseBlock(b, Args.ValuePath);
                if (result == null || !(result is IValueBlock)) return null;

                return (result as IValueBlock)?.Value;
            }).OfType<string>().ToArray();
        }
    }
}
