using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Reading.Regex
{
    public class RangeStep : ValidationStep
    {
        public readonly char FirstChar;
        public readonly char LastChar;

        public RangeStep(char start, char end)
            : base()
        {
            FirstChar = start;
            LastChar = end;
        }

        protected override bool ValidateCharacter(CharInfo charInfo)
        {
            return FirstChar <= charInfo.CurrentChar && charInfo.CurrentChar <= LastChar;
        }

        protected override RegexInfo CreateInfo(RegexInfo? parent, CharInfo charInfo)
        {
            return new RangeInfo(
                this,
                parent,
                parent?.Start ?? charInfo.CurrentPosition
            );
        }

        private class RangeInfo : RegexInfo<RangeStep>
        {
            public RangeInfo(RangeStep step, RegexInfo? parent, CharacterPosition start)
                : base(step, parent, start) { }

            public override RegexInfo[] DetermainNext(CharInfo charInfo)
            {
                if (Parent == null)
                {
                    return new RegexInfo[0];
                }
                return Parent.DetermainNext(charInfo);
            }
        }
    }
}
