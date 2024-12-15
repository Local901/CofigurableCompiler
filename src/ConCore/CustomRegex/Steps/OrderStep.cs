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
        public OrderStep(List<RegexStep<NextInput, Result>> childSteps)
            : base(childSteps) { }

        public override RegexInfo<NextInput, Result>[] Start(NextInput charInfo)
        {
            return DetermainNext(null, charInfo);
        }

        public override IValueInfo<NextInput, Result>[] DetermainNext(
            RegexInfo<NextInput, Result>? parent,
            NextInput value
        )
        {
            List<IValueInfo<NextInput, Result>> result = new List<IValueInfo<NextInput, Result>>();

            if (Optional || ChildSteps.Count == 0)
            {
                if (parent == null)
                {
                    var completeInfo = new EndInfo<NextInput, Result>();
                    result.Add(completeInfo);
                }
                else
                {
                    result.AddRange(parent.DetermainNext(value));
                }
            }

            if (ChildSteps.Count == 0)
            {
                return result.ToArray();
            }

            OrderInfo orderInfo = new OrderInfo(this, parent);
            ChildSteps[0].DetermainNext(orderInfo, value);

            return result.ToArray();
        }

        private class OrderInfo : RegexInfo<NextInput, Result, OrderStep<NextInput, Result>>
        {
            private int Index;

            public OrderInfo(
                OrderStep<NextInput, Result> step,
                RegexInfo<NextInput, Result>? parent,
                int index = 0
            ) : base(step, parent)
            {
                Index = index;
            }

            public override IValueInfo<NextInput, Result>[] DetermainNext(NextInput value)
            {
                if (Index >= CurrentStep.ChildSteps.Count)
                {
                    if (Parent == null)
                    {
                        return new IValueInfo<NextInput, Result>[0];
                    }
                    return Parent.DetermainNext(value);
                }

                OrderInfo nextInfo = new OrderInfo(CurrentStep, Parent, Index + 1);
                return CurrentStep.ChildSteps[Index].DetermainNext(nextInfo, value);
            }
        }
    }
}
