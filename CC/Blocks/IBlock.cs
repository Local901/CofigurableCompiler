using System;
using System.Collections.Generic;
using System.Text;
using CC.Key;

namespace CC.Blocks
{
    public interface IBlock
    {
        IKey Key { get; }
        string Name { get; }
        int Index { get; }
        int EndIndex { get; }

        IBlock Copy(string name = null);
    }
}
