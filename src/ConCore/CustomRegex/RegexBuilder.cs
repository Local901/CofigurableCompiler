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

    public class RegexBuilder<Context, Result>
    {
        public RegexStep<Context, Result> Ordered(bool optional = false, params RegexStep<Context, Result>[] steps)
        {
            return Ordered(steps, optional);
        }
        public RegexStep<Context, Result> Ordered(RegexStep<Context, Result>[] steps, bool optional = false)
        {
            var step = new OrderStep<Context, Result>(steps);
            step.Optional = step.Optional || optional;
            return step;
        }

        public RegexStep<Context, Result> Any(bool optional, params RegexStep<Context, Result>[] steps)
        {
            return Any(steps, optional);
        }
        public RegexStep<Context, Result> Any(RegexStep<Context, Result>[] steps, bool optional = false)
        {
            var step = new AnyStep<Context, Result>(steps);
            step.Optional = step.Optional || optional;
            return step;
        }

        public RegexStep<Context, Result> Repeat(RegexStep<Context, Result> step, int maximum = 0, int minimum = 0)
        {
            return new RepeatStep<Context, Result>(step, minimum, maximum);
        }

        public RegexStep<Context, Result> Optional(RegexStep<Context, Result> step)
        {
            return new OptionalStep<Context, Result>(step);
        }

        public RegexStep<Context, Result> SeparatedList(
            RegexStep<Context, Result> main,
            RegexStep<Context, Result> separator,
            bool optional = false,
            SeparatorOptions allowEndingSeparator = SeparatorOptions.OPTIONAL,
            int minimum = 0,
            int maximum = 0
        )
        {
            var steps = new List<RegexStep<Context, Result>>() { main };

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

        public RegexStep<Context, Result> Value(Result value)
        {
            return new ValueStep<Context, Result>(value);
        }
    }
}
