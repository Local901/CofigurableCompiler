using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Contract
{
    public interface IBlock
    {
        IKey Key { get; set; }
        string Name { get; set; }
        string Value { get; set; }
        int Index { get; set; }
        int EndIndex { get; set; }

        List<IBlock> Content { get; set; }
        IBlock Parent { get; set; }
    }
}
