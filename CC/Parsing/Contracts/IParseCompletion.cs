﻿using CC.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parsing.Contracts
{
    public interface IParseCompletion
    {
        public int Round { get; }
        public IBlock Block { get; }
    }
}