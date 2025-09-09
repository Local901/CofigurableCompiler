using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.CustomRegex.Info;

namespace ConCore.CustomRegex.Steps
{
    internal class OptionalStep<Context, Result> : RegexStep<Context, Result>
    {
        public OptionalStep(RegexStep<Context, Result> nextStep)
            : base([nextStep])
        { }

        public override IList<IValueInfo<Context, Result>?> Start(Context value)
        {
            return ChildSteps[0].Start(value).Append(null).ToArray();
        }

        public override IList<IValueInfo<Context, Result>?> DetermainNext(RegexInfo<Context, Result>? parent, Context value)
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
