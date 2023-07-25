using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing
{
    [Flags]
    public enum ParseStatus
    {
        None,
        CanEnd,
        ReachedEnd,
        IsComplete,
        Error
    }
}
