using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Reading.Regex
{
    public class AnyStep : RegexStep
    {
        public AnyStep(List<RegexStep> childSteps)
            : base(childSteps) { }

        public override RegexInfo[] Start(CharInfo charInfo)
        {
            return DetermainNext(null, charInfo);
        }

        public override RegexInfo[] DetermainNext(RegexInfo? parent, CharInfo charInfo)
        {
            List<RegexInfo> result = new List<RegexInfo>();

            if (Optional || ChildSteps.Count == 0)
            {
                if (parent != null)
                {
                    result.AddRange(parent.DetermainNext(charInfo));
                } else
                {
                    CharacterPosition start = parent?.Start ?? charInfo.CurrentPosition;
                    result.Add(new EndInfo(
                        start,
                        start.Index > charInfo.PreviousPosition.Index ? start : charInfo.PreviousPosition
                    ));
                }
            }

            result.AddRange(ChildSteps.SelectMany((step) => step.DetermainNext(parent, charInfo)));
            return result.ToArray();
        }
    }
}
