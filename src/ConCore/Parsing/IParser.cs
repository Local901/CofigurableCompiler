using ConCore.Blocks;
using ConCore.Key;

namespace ConCore.Parsing
{
    public interface IParser
    {
        /// <summary>
        /// Parse using the FileLexer and output the a block with the created tree.
        /// </summary>
        /// <param name="startConstruct"></param>
        IBlock? DoParse(KeyLangReference startConstruct);
    }
}
