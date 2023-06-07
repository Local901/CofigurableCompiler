using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing
{
    internal class ConstructParseResult : IConstructParseResult
    {
        public bool IsUsed { get; set; }

        public bool IsComplete { get; set; }

        public bool CanContinue { get; set; }

        public bool IsIgnored { get; set; }
    }
}
