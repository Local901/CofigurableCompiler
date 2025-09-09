using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Blocks;
using ConCore.CustomRegex.Steps;

namespace ConCore.CustomRegex.Info
{
    public abstract class RegexInfo<Context, Result>
    {
        protected RegexInfo() { }

        /// <summary>
        /// Determine next step based on the character info.
        /// </summary>
        /// <param name="value">The current value</param>
        /// <returns></returns>
        public abstract IList<IValueInfo<Context, Result>?> DetermainNext(Context value);
    }

    public abstract class RegexInfo<Context, Result, T> : RegexInfo<Context, Result>
        where T : RegexStep<Context, Result>
    {
        protected T CurrentStep { get; }
        protected RegexInfo<Context, Result>? Parent { get; }

        protected RegexInfo(T currentStep, RegexInfo<Context, Result>? parent)
            : base()
        {
            CurrentStep = currentStep;
            Parent = parent;
        }
    }
}
