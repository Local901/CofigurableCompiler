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
        public readonly ReadCondition? PrecedingCondition;

        public LexOptions(KeyLangReference key, ReadCondition? precedingCondition = null)
        {
            Key = key;
            PrecedingCondition = precedingCondition;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is not LexOptions other) return false;
            return Key == other.Key && PrecedingCondition == other.PrecedingCondition;
        }

        public static bool operator ==(LexOptions? left, LexOptions? right)
        {
            if (Object.ReferenceEquals(left, null))
            {
                if (Object.ReferenceEquals(right, null))
                {
                    return true;
                }
                return false;
            }
            return left.Equals(right);
        }
        public static bool operator !=(LexOptions? left, LexOptions? right)
        {
            return !(left == right);
        }
    }
}
