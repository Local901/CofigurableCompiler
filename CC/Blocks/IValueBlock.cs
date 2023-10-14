using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Blocks
{
    public interface IValueBlock : IBlock
    {
        string Value { get; }
    }
}
