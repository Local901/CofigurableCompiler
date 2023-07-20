using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Contract
{
    public interface IBlock
    {
        IKey Key { get; }
        string Name { get; }
        string Value { get; }
        int Index { get;}
        int EndIndex { get;}
    }
}
