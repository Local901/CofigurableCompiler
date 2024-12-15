using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.CustomRegex.Info;

namespace ConCore.CustomRegex.Steps
{
    internal class OptionalStep<NextInput, Result> : RegexStep<NextInput, Result>
    {
        private readonly RegexStep<NextInput, Result> NextStep;

        public OptionalStep(RegexStep<NextInput, Result> nextStep)
        {
            NextStep = nextStep;
        }

        public override RegexInfo<NextInput, Result>[] Start(NextInput value)
        {
            return NextStep.Start(value).Append(new EndInfo<NextInput, Result>()).ToArray();
        }

        public override IValueInfo<NextInput, Result>[] DetermainNext(RegexInfo<NextInput, Result>? parent, NextInput value)
        {
            if (parent == null)
            {
                return NextStep.DetermainNext(parent, value)
                    .Append(new EndInfo<NextInput, Result>())
                    .ToArray();
            }
            return NextStep.DetermainNext(parent, value)
                .Concat(parent.DetermainNext(value))
                .ToArray();
        }
    }
}
