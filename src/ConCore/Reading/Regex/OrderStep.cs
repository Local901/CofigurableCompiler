using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Reading.Regex
{
    public class OrderStep : RegexStep
    {
        public OrderStep(List<RegexStep> childSteps)
            : base(childSteps) { }

        public override RegexInfo[] Start(CharInfo charInfo)
        {
            return DetermainNext(null, charInfo);
        }

        public override RegexInfo[] DetermainNext(RegexInfo? parent, CharInfo charInfo)
        {
            List<RegexInfo> result = new List<RegexInfo>();

            if (Optional || ChildSteps.Count == 0)
            {
                if (parent == null)
                {
                    var completeInfo = new OrderInfo(this, parent, parent?.Start ?? charInfo.CurrentPosition);
                    completeInfo.ReachedEnd(
                        completeInfo.Start.Index > charInfo.PreviousPosition.Index
                            ? charInfo.CurrentPosition
                            : charInfo.PreviousPosition
                    );
                    result.Add(completeInfo);
                } else {
                    result.AddRange(parent.DetermainNext(charInfo));
                }
            }

            if (ChildSteps.Count == 0)
            {
                return result.ToArray();
            }

            OrderInfo orderInfo = new OrderInfo(this, parent, parent?.Start ?? charInfo.CurrentPosition);
            ChildSteps[0].DetermainNext(orderInfo, charInfo);

            return result.ToArray();
        }

        private class OrderInfo : RegexInfo<OrderStep>
        {
            private int Index;

            public OrderInfo(OrderStep step, RegexInfo? parent, CharacterPosition start, int index = 0)
                : base(step, parent, start)
            {
                Index = index;
            }

            public override RegexInfo[] DetermainNext(CharInfo charInfo)
            {
                if (Index >= CurrentStep.ChildSteps.Count)
                {
                    if (Parent == null)
                    {
                        ReachedEnd(charInfo.PreviousPosition);
                        return new RegexInfo[] { this };
                    }
                    return Parent.DetermainNext(charInfo);
                }

                OrderInfo nextInfo = new OrderInfo(CurrentStep, Parent, Start, Index + 1);
                return CurrentStep.ChildSteps[Index].DetermainNext(nextInfo, charInfo);
            }
        }
    }
}
