using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Blocks;
using ConCore.CustomRegex.Info;

namespace ConCore.CustomRegex.Steps
{
    internal class AnyStep<Context, Result> : RegexStep<Context, Result>
    {
        public AnyStep(IList<RegexStep<Context, Result>> childSteps)
            : base(childSteps) { }

        public override IList<IValueInfo<Context, Result>?> Start(Context value)
        {
            return DetermainNext(null, value);
        }

        public override IList<IValueInfo<Context, Result>?> DetermainNext(RegexInfo<Context, Result>? parent, Context value)
        {
            var result = new List<IValueInfo<Context, Result>?>();

            if (Optional || ChildSteps.Count == 0)
            {
                if (parent != null)
                {
                    result.AddRange(parent.DetermainNext(value));
                }
                else
                {
                    // This step is optional or doesn't check anything.
                    result.Add(null);
                }
            }

            result.AddRange(ChildSteps.SelectMany((step) => step.DetermainNext(parent, value)));
            return result;
        }
    }
}
