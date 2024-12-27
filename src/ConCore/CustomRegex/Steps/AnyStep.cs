using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Blocks;
using ConCore.CustomRegex.Info;

namespace ConCore.CustomRegex.Steps
{
    internal class AnyStep<NextInput, Result> : RegexStep<NextInput, Result>
    {
        public AnyStep(List<RegexStep<NextInput, Result>> childSteps)
            : base(childSteps) { }

        public override IList<IValueInfo<NextInput, Result>?> Start(NextInput value)
        {
            return DetermainNext(null, value);
        }

        public override IList<IValueInfo<NextInput, Result>?> DetermainNext(RegexInfo<NextInput, Result>? parent, NextInput value)
        {
            var result = new List<IValueInfo<NextInput, Result>?>();

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
