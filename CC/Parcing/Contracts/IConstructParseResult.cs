using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public interface IConstructParseResult
    {
        public bool IsUsed { get; }
        public bool IsComplete { get; }
        public bool CanContinue { get; }
        public bool IsIgnored { get; }
    }
}
