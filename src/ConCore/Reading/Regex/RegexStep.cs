using System;
using System.Collections.Generic;
using System.Linq;

namespace ConCore.Reading.Regex
{
    public abstract class RegexStep
    {
        public List<RegexStep> ChildSteps { get; }

        // Step options
        public bool Optional { get; set; } = false;
        // End step options

        protected RegexStep(List<RegexStep> childSteps)
        {
            ChildSteps = childSteps;
        }
        protected RegexStep()
            : this(new List<RegexStep>()) { }

        public abstract RegexInfo[] Start(CharInfo charInfo);
        public abstract RegexInfo[] DetermainNext(RegexInfo? parent, CharInfo charInfo);
    }
}
