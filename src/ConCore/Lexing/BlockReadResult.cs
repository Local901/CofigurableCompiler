using ConCore.Blocks;
using ConCore.Key;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Lexing
{
    public struct BlockReadResult
    {
        public IKey Key { get; set; }
        public string MatchValue { get; set; }
        public string PrecedingValue { get; set; }
        public CharacterPosition MatchStart { get; set; }
        public CharacterPosition MatchEnd { get; set; }
        public CharacterPosition PrecedingStart { get; set; }
    }
}
