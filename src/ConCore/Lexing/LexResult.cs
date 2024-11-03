using ConCore.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Lexing
{
    public struct LexResult
    {
        public IValueBlock PrecedingBlock { get; set; }
        public IValueBlock Block { get; set; }
    }
}
