using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.CustomRegex.Info;

namespace ConCore.CustomRegex.Steps
{
    public abstract class ValidationStep<NextInput, Result> : RegexStep<NextInput, Result>
    {
        public ValidationStep(List<RegexStep<NextInput, Result>> nextSteps)
            : base(nextSteps) { }
        public ValidationStep()
            : base() { }

        public sealed override IList<IValueInfo<NextInput, Result>?> Start(NextInput value)
        {
            return DetermainNext(null, value);
        }
        public sealed override IList<IValueInfo<NextInput, Result>?> DetermainNext(
            RegexInfo<NextInput, Result>? parent,
            NextInput value
        )
        {
            var result = new List<IValueInfo<NextInput, Result>?>();
            if (parent != null && Optional)
            {
                result.AddRange(parent.DetermainNext(value));
            }
            if (ValidateValue(value))
            {
                result.Add(CreateInfo(parent, value));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Validate the value to check if it matches a rule.
        /// 
        /// If true info will bw created and returned to the caller.
        /// </summary>
        /// <param name="value">Value to check with.</param>
        /// <returns></returns>
        protected abstract bool ValidateValue(NextInput value);
        /// <summary>
        /// Create a info object
        /// </summary>
        /// <param name="parent">The parent info object that called this step.</param>
        /// <param name="value">The value to send back.</param>
        /// <returns></returns>
        protected abstract IValueInfo<NextInput, Result> CreateInfo(RegexInfo<NextInput, Result>? parent, NextInput value);
    }
}
