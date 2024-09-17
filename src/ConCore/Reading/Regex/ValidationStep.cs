using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Reading.Regex
{
    public abstract class ValidationStep : RegexStep
    {
        public ValidationStep(List<RegexStep> nextSteps)
            : base(nextSteps) { }
        public ValidationStep()
            : base() { }

        public sealed override RegexInfo[] Start(CharInfo charInfo)
        {
            return DetermainNext(null, charInfo);
        }
        public sealed override RegexInfo[] DetermainNext(RegexInfo? parent, CharInfo charInfo)
        {
            List<RegexInfo> result = new List<RegexInfo>();
            if (parent != null && Optional)
            {
                result.AddRange(parent.DetermainNext(charInfo));
            }
            if (ValidateCharacter(charInfo))
            {
                var info = CreateInfo(parent, charInfo);
                if (parent == null)
                {
                    info.ReachedEnd(charInfo.CurrentPosition);
                }
                result.Add(info);
            }
            return result.ToArray();
        }

        protected abstract bool ValidateCharacter(CharInfo charInfo);
        protected abstract RegexInfo CreateInfo(RegexInfo? parent, CharInfo charInfo);
    }
}
