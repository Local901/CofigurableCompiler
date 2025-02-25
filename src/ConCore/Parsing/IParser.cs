using ConCore.Blocks;
using ConCore.Key;
using System;

namespace ConCore.Parsing
{
    public interface IParser
    {
        /// <summary>
        /// Parse using the FileLexer and output the a block with the created tree.
        /// </summary>
        /// <param name="startConstruct"></param>
        [Obsolete("Parser will be instanced. (New Function Parse)")]
        IBlock? DoParse(KeyLangReference startConstruct);

        IBlock? Parse();
    }
}
