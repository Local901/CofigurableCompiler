using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Reading.Regex
{
    public class EndInfo : RegexInfo
    {
        public EndInfo(CharacterPosition start, CharacterPosition end)
            : base(start)
        {
            ReachedEnd(end);
        }

        public override RegexInfo[] DetermainNext(CharInfo charInfo)
        {
            return new RegexInfo[0];
        }
    }
}
