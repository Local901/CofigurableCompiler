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
        public OptionalStep(RegexStep<NextInput, Result> nextStep)
            : base(new RegexStep<NextInput, Result>[] { nextStep })
        { }

        public override IList<IValueInfo<NextInput, Result>?> Start(NextInput value)
        {
            return ChildSteps[0].Start(value).Append(null).ToArray();
        }

        public override IList<IValueInfo<NextInput, Result>?> DetermainNext(RegexInfo<NextInput, Result>? parent, NextInput value)
        {
            if (parent == null)
            {
                return ChildSteps[0].DetermainNext(parent, value)
                    .Append(null)
                    .ToList();
            }
            return ChildSteps[0].DetermainNext(parent, value)
                .Concat(parent.DetermainNext(value))
                .ToList();
        }
    }
}
