using System;

namespace ConCore.Parsing.Simple
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
