using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.CustomRegex.Info;

namespace ConCore.CustomRegex.Steps
{
    public abstract class ValidationStep<Context, Result> : RegexStep<Context, Result>
    {
        public ValidationStep()
            : base() { }

        public sealed override IList<IValueInfo<Context, Result>?> Start(Context value)
        {
            return DetermainNext(null, value);
        }
        public sealed override IList<IValueInfo<Context, Result>?> DetermainNext(
            RegexInfo<Context, Result>? parent,
            Context value
        )
        {
            var result = new List<IValueInfo<Context, Result>?>();
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
        protected abstract bool ValidateValue(Context value);
        /// <summary>
        /// Create a info object
        /// </summary>
        /// <param name="parent">The parent info object that called this step.</param>
        /// <param name="value">The value to send back.</param>
        /// <returns></returns>
        protected abstract IValueInfo<Context, Result> CreateInfo(RegexInfo<Context, Result>? parent, Context value);
    }
}
