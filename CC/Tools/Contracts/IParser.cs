using CC.Blocks;
using CC.Key;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Tools.Contracts
{
    public interface IParser
    {
        /// <summary>
        /// Parse using the FileLexer and output the a block with the created tree.
        /// </summary>
        /// <param name="startConstruct"></param>
        IBlock DoParse(KeyLangReference startConstruct);
    }
}
