using CC.Contract;
using CC.Parcing.ComponentTypes;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CC.Parcing
{
    public class IssueBlock : Block, IIssueBlock
    {
        public IssueType Type { get; set; }

        public IBlock Copy(string name = null)
        {
            return new IssueBlock
            {
                Key = Key,
                Name = name == null ? Name : name,
                Type = Type,
                Value = Value,
                Index = Index,
                EndIndex = EndIndex
            };
        }
    }
}
