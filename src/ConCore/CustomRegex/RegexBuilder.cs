using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.CustomRegex.Steps;

namespace ConCore.CustomRegex
{
    public enum SeparatorOptions
    {
        NEVER,
        OPTIONAL,
        ALWAYS
    }

    public class RegexBuilder<NextInput, Result>
    {
        public RegexStep<NextInput, Result> Ordered(bool optional = false, params RegexStep<NextInput, Result>[] steps)
        {
            return Ordered(steps, optional);
        }
        public RegexStep<NextInput, Result> Ordered(RegexStep<NextInput, Result>[] steps, bool optional = false)
        {
            var step = new OrderStep<NextInput, Result>(steps);
            step.Optional = step.Optional || optional;
            return step;
        }

        public RegexStep<NextInput, Result> Any(bool optional, params RegexStep<NextInput, Result>[] steps)
        {
            return Any(steps, optional);
        }
        public RegexStep<NextInput, Result> Any(RegexStep<NextInput, Result>[] steps, bool optional = false)
        {
            var step = new AnyStep<NextInput, Result>(steps);
            step.Optional = step.Optional || optional;
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
            SeparatorOptions allowEndingSeparator = SeparatorOptions.OPTIONAL,
            int minimum = 0,
            int maximum = 0
        )
        {
            var steps = new List<RegexStep<NextInput, Result>>() { main };

            // Don't add the repeat of only one element is allowed.
            if ( maximum == 0 || maximum > 1)
            {
                steps.Add(
                    Repeat(
                        Ordered(false,
                            separator,
                            main
                        ),
                        maximum - 1,
                        minimum - 1
                    )
                );
            }
            switch(allowEndingSeparator)
            {
                case SeparatorOptions.NEVER:
                    break;
                case SeparatorOptions.OPTIONAL:
                    steps.Add(Optional(separator));
                    break;
                case SeparatorOptions.ALWAYS:
                    steps.Add(separator);
                    break;
            }
            return Ordered(steps.ToArray(), optional || minimum == 0);
        }

        public RegexStep<NextInput, Result> Value(Result value)
        {
            return new ValueStep<NextInput, Result>(value);
        }
    }
}
