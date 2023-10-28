﻿using CC.Blocks;
using CC.Key;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC
{
    public interface IParser
    {
        /// <summary>
        /// Parse using the FileLexer and output the a block with the created tree.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="startConstruct"></param>
        void DoParse(out IBlock block, KeyLangReference startConstruct);
    }
}