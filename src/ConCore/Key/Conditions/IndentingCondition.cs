using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConCore.Key.Conditions
{
    public class IndentingCondition : RegexCondition
    {
        public readonly ConditionArgs Args;

        public IndentingCondition(ConditionArgs args)
            :base(new Regex(".*^(?:" + args.indentSnippets.Aggregate((s1, s2) => $"{s1}|{s2}") + "){" + args.depth + "}", RegexOptions.Multiline))
        {
            Args = args;
        }
    }
}
