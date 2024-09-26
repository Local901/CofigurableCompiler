using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Blocks;

namespace ConCore.Reading.Regex
{
    public class RepeatStep : RegexStep
    {
        public int MinimumRepeats { get; }
        public int MaximumRepeats { get; }

        public RepeatStep(RegexStep childStep, int minimum = 0, int maximum = 0)
            : base(new List<RegexStep>() { childStep })
        {
            if (childStep.Optional)
            {
                throw new ArgumentException("The child step of a repeat step can't be optional.");
            }

            MinimumRepeats = minimum;
            MaximumRepeats = maximum;

            if (minimum < 1)
            {
                Optional = true;
            }
        }

        public override RegexInfo[] Start(CharInfo charInfo)
        {
            return DetermainNext(null, charInfo);
        }

        public override RegexInfo[] DetermainNext(RegexInfo? parent, CharInfo charInfo)
        {
            return new RepeatInfo(this, parent, parent?.Start ?? charInfo.CurrentPosition).DetermainNext(charInfo);
        }

        private class RepeatInfo : RegexInfo<RepeatStep>
        {
            public int RepeatIndex { get; }

            public RepeatInfo(RepeatStep step, RegexInfo? parent, CharacterPosition start, int repeat = 0)
                : base(step, parent, start)
            {
                RepeatIndex = repeat;
            }

            public override RegexInfo[] DetermainNext(CharInfo charInfo)
            {
                List<RegexInfo> result = new List<RegexInfo>();

                if (RepeatIndex >= CurrentStep.MinimumRepeats)
                {
                    if (Parent == null)
                    {
                        ReachedEnd(Start.Index > charInfo.PreviousPosition.Index ? Start : charInfo.PreviousPosition);
                        result.Add(this);
                    } else
                    {
                        result.AddRange(Parent.DetermainNext(charInfo));
                    }
                }

                if (RepeatIndex < CurrentStep.MaximumRepeats)
                {
                    RepeatInfo nextInfo = new RepeatInfo(CurrentStep, Parent, Start, RepeatIndex + 1);
                    result.AddRange(CurrentStep.ChildSteps[0].DetermainNext(nextInfo, charInfo));
                }

                return result.ToArray();
            }
        }
    }
}
