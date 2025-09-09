using System;
using System.Collections.Generic;
using System.Linq;
using ConCore.CustomRegex.Info;

namespace ConCore.CustomRegex.Steps
{
    public abstract class RegexStep<Context, Result>
    {
        public IList<RegexStep<Context, Result>> ChildSteps { get; }

        // Step options
        public bool Optional { get; set; } = false;
        // End step options

        protected RegexStep(IList<RegexStep<Context, Result>> childSteps)
        {
            ChildSteps = childSteps;
        }
        protected RegexStep()
            : this(new List<RegexStep<Context, Result>>()) { }

        /// <summary>
        /// Start the regex from this step.
        /// </summary>
        /// <param name="value">The first value.</param>
        /// <returns>Info objects that matched the value.</returns>
        public abstract IList<IValueInfo<Context, Result>?> Start(Context value);
        /// <summary>
        /// Determine all next info that match the value.
        /// </summary>
        /// <param name="parent">The parent info to callback to when step is finished.</param>
        /// <param name="value">The value to check.</param>
        /// <returns>Info objects that match the value.</returns>
        public abstract IList<IValueInfo<Context, Result>?> DetermainNext(RegexInfo<Context, Result>? parent, Context value);
    }
}
