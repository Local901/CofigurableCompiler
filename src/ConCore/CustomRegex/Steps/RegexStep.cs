using System;
using System.Collections.Generic;
using System.Linq;
using ConCore.CustomRegex.Info;

namespace ConCore.CustomRegex.Steps
{
    public abstract class RegexStep<NextInput, Result>
    {
        public List<RegexStep<NextInput, Result>> ChildSteps { get; }

        // Step options
        public bool Optional { get; set; } = false;
        // End step options

        protected RegexStep(List<RegexStep<NextInput, Result>> childSteps)
        {
            ChildSteps = childSteps;
        }
        protected RegexStep()
            : this(new List<RegexStep<NextInput, Result>>()) { }

        /// <summary>
        /// Start the regex from this step.
        /// </summary>
        /// <param name="value">The first value.</param>
        /// <returns>Info objects that matched the value.</returns>
        public abstract RegexInfo<NextInput, Result>[] Start(NextInput value);
        /// <summary>
        /// Determine all next info that match the value.
        /// </summary>
        /// <param name="parent">The parent info to callback to when step is finished.</param>
        /// <param name="value">The value to check.</param>
        /// <returns>Info objects that match the value.</returns>
        public abstract IValueInfo<NextInput, Result>[] DetermainNext(RegexInfo<NextInput, Result>? parent, NextInput value);
    }
}
