using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Blocks;
using ConCore.CustomRegex.Info;

namespace ConCore.CustomRegex.Steps
{
    internal class OrderStep<NextInput, Result> : RegexStep<NextInput, Result>
    {
        public OrderStep(IList<RegexStep<NextInput, Result>> childSteps)
            : base(childSteps)
        {
            if (childSteps.Count == 0)
            {
                Optional = true;
            }
        }

        public override IList<IValueInfo<NextInput, Result>?> Start(NextInput charInfo)
        {
            return DetermainNext(null, charInfo);
        }

        public override IList<IValueInfo<NextInput, Result>?> DetermainNext(
            RegexInfo<NextInput, Result>? parent,
            NextInput value
        )
        {
            var result = new List<IValueInfo<NextInput, Result>?>();

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

        private class OrderInfo : RegexInfo<NextInput, Result, OrderStep<NextInput, Result>>
        {
            public readonly int Index;
            private OrderInfo? nextInfo;

            public OrderInfo(
                OrderStep<NextInput, Result> step,
                RegexInfo<NextInput, Result>? parent,
                int index = 0
            ) : base(step, parent)
            {
                Index = index;
            }

            public override IList<IValueInfo<NextInput, Result>?> DetermainNext(NextInput value)
            {
                if (Index >= CurrentStep.ChildSteps.Count)
                {
                    if (Parent == null)
                    {
                        return new List<IValueInfo<NextInput, Result>?>() { null };
                    }
                    return Parent.DetermainNext(value);
                }

                nextInfo ??= new OrderInfo(CurrentStep, Parent, Index + 1);
                return CurrentStep.ChildSteps[Index].DetermainNext(nextInfo, value);
            }
        }
    }
}
