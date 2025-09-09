using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Blocks;
using ConCore.CustomRegex.Info;

namespace ConCore.CustomRegex.Steps
{
    internal class OrderStep<Context, Result> : RegexStep<Context, Result>
    {
        public OrderStep(IList<RegexStep<Context, Result>> childSteps)
            : base(childSteps)
        {
            if (childSteps.Count == 0)
            {
                Optional = true;
            }
        }

        public override IList<IValueInfo<Context, Result>?> Start(Context charInfo)
        {
            return DetermainNext(null, charInfo);
        }

        public override IList<IValueInfo<Context, Result>?> DetermainNext(
            RegexInfo<Context, Result>? parent,
            Context value
        )
        {
            var result = new List<IValueInfo<Context, Result>?>();

            if (Optional)
            {
                if (parent == null)
                {
                    result.Add(null);
                }
                else
                {
                    result.AddRange(parent.DetermainNext(value));
                }
            }

            if (ChildSteps.Count == 0)
            {
                return result;
            }

            OrderInfo orderInfo = new OrderInfo(this, parent);
            result.AddRange(orderInfo.DetermainNext(value));
            return result;
        }

        private class OrderInfo : RegexInfo<Context, Result, OrderStep<Context, Result>>
        {
            public readonly int Index;
            private OrderInfo? nextInfo;

            public OrderInfo(
                OrderStep<Context, Result> step,
                RegexInfo<Context, Result>? parent,
                int index = 0
            ) : base(step, parent)
            {
                Index = index;
            }

            public override IList<IValueInfo<Context, Result>?> DetermainNext(Context value)
            {
                if (Index >= CurrentStep.ChildSteps.Count)
                {
                    if (Parent == null)
                    {
                        return new List<IValueInfo<Context, Result>?>() { null };
                    }
                    return Parent.DetermainNext(value);
                }

                nextInfo ??= new OrderInfo(CurrentStep, Parent, Index + 1);
                return CurrentStep.ChildSteps[Index].DetermainNext(nextInfo, value);
            }
        }
    }
}
