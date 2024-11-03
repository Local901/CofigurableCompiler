using ConCore.Key;
using ConCore.Lexing.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Lexing
{
    public struct LexOptions
    {
        public readonly KeyLangReference Key;
        public readonly ReadCondition PrecedingCondition;
    }
}
