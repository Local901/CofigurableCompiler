using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Blocks;
using ConCore.CustomRegex.Info;

namespace ConCore.CustomRegex.Steps
{
    internal class RepeatStep<Context, Result> : RegexStep<Context, Result>
    {
        public int MinimumRepeats { get; }
        public int MaximumRepeats { get; }

        public RepeatStep(RegexStep<Context, Result> childStep, int minimum = 0, int maximum = 0)
            : base([childStep])
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

        public override IList<IValueInfo<Context, Result>?> Start(Context value)
        {
            return DetermainNext(null, value);
        }

        public override IList<IValueInfo<Context, Result>?> DetermainNext(
            RegexInfo<Context, Result>? parent,
            Context value
        )
        {
            return new RepeatInfo(this, parent).DetermainNext(value);
        }

        private class RepeatInfo : RegexInfo<Context, Result, RepeatStep<Context, Result>>
        {
            public readonly int RepeatIndex;
            private RepeatInfo? nextInfo;

            public RepeatInfo(RepeatStep<Context, Result> step, RegexInfo<Context, Result>? parent, int repeat = 0)
                : base(step, parent)
            {
                RepeatIndex = repeat;
            }

            public override IList<IValueInfo<Context, Result>?> DetermainNext(Context value)
            {
                var result = new List<IValueInfo<Context, Result>?>();

                if (RepeatIndex >= CurrentStep.MinimumRepeats)
                {
                    if (Parent == null)
                    {
                        // If it end is reached before any iteration is completed
                        result.Add(null);
                    }
                    else
                    {
                        result.AddRange(Parent.DetermainNext(value));
                    }
                }

                if (RepeatIndex < CurrentStep.MaximumRepeats || CurrentStep.MaximumRepeats <= 0)
                {
                    nextInfo ??= new RepeatInfo(CurrentStep, Parent, RepeatIndex + 1);
                    result.AddRange(CurrentStep.ChildSteps[0].DetermainNext(nextInfo, value));
                }

                return result;
            }
        }
    }
}
