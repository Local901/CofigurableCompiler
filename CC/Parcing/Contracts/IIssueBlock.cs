using CC.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public interface IIssueBlock : IBlock
    {
        public IssueType Type { get; }
    }
}
