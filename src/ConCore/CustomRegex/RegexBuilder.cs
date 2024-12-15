using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.CustomRegex.Steps;

namespace ConCore.CustomRegex
{
    public class RegexBuilder<NextInput, Result>
    {
        public RegexStep<NextInput, Result> Ordered(bool optional, params RegexStep<NextInput, Result>[] steps)
        {
            return Ordered(steps, optional);
        }
        public RegexStep<NextInput, Result> Ordered(RegexStep<NextInput, Result>[] steps, bool optional = false)
        {
            var step = new OrderStep<NextInput, Result>(steps.ToList());
            step.Optional = optional;
            return step;
        }

        public RegexStep<NextInput, Result> Any(bool optional, params RegexStep<NextInput, Result>[] steps)
        {
            return Any(steps, optional);
        }
        public RegexStep<NextInput, Result> Any(RegexStep<NextInput, Result>[] steps, bool optional = false)
        {
            var step = new AnyStep<NextInput, Result>(steps.ToList());
            step.Optional = optional;
            return step;
        }

        public RegexStep<NextInput, Result> Repeat(RegexStep<NextInput, Result> step, int maximum = 0, int minimum = 0)
        {
            return new RepeatStep<NextInput, Result>(step, minimum, maximum);
        }

        public RegexStep<NextInput, Result> Optional(RegexStep<NextInput, Result> step)
        {
            return new OptionalStep<NextInput, Result>(step);
        }

        public RegexStep<NextInput, Result> SeparatedList(
            RegexStep<NextInput, Result> main,
            RegexStep<NextInput, Result> separator,
            bool optional = false,
            bool allowEndingSeparator = true
        )
        {
            var result = Ordered(optional,
                main,
                Repeat(
                    Ordered(false,
                        separator,
                        main
                    )
                )
            );
            if (allowEndingSeparator)
            {
                return Ordered(optional, result, Optional(separator));
            }
            return result;
        }

        public RegexStep<NextInput, Result> Value(Result value)
        {
            return new ValueStep<NextInput, Result>(value);
        }
    }
}
