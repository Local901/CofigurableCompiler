using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Reading.Regex
{
    public class LineEndStep : RegexStep
    {
        public LineEndStep()
            : base() { }

        public override RegexInfo[] Start(CharInfo charInfo)
        {
            return DetermainNext(null, charInfo);
        }

        public override RegexInfo[] DetermainNext(RegexInfo? parent, CharInfo charInfo)
        {
            if (parent == null)
            {
                return new RegexInfo[] { new EndInfo(charInfo.CurrentPosition, charInfo.PreviousPosition) };
            }
            if (charInfo.CurrentChar == '\n' || Optional)
            {
                return parent.DetermainNext(charInfo);
            }
            return new RegexInfo[0];
        }
    }
}
