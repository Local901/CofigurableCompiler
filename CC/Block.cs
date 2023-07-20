using CC.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC
{
    public class Block : IBlock
    {
        public IKey Key { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int Index { get; set; }
        public int EndIndex { get; set; }
    }
}
