using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parsing
{
    [Flags]
    public enum ParseStatus
    {
        None,
        CanEnd,
        ReachedEnd,
        IsComplete,
        Error,
        ExpectedSomethingElse
    }
}
