using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Key.Conditions
{
    public struct ConditionArgs
    {
        public int depth = 0;
        public string[] indentSnippets = ["\t"];

        public ConditionArgs() { }
    }
}
