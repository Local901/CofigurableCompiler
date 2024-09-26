using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Blocks;

namespace ConCore.Reading.Regex
{
    public class CharStep : ValidationStep
    {
        public char Character;

        public CharStep(char character)
            : base()
        {
            Character = character;
        }

        protected override bool ValidateCharacter(CharInfo charInfo)
        {
            return charInfo.CurrentChar == Character;
        }

        protected override RegexInfo CreateInfo(RegexInfo? parent, CharInfo charInfo)
        {
            return new CharStepInfo(
                this,
                parent,
                parent?.Start ?? charInfo.CurrentPosition
            );
        }

        private class CharStepInfo : RegexInfo<CharStep>
        {
            public CharStepInfo(CharStep step, RegexInfo? parent, CharacterPosition start)
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
