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
    public abstract class RegexInfo<NextInput, Result>
    {
        protected RegexInfo() { }

        /// <summary>
        /// Determine next step based on the character info.
        /// </summary>
        /// <param name="value">The current value</param>
        /// <returns></returns>
        public abstract IList<IValueInfo<NextInput, Result>?> DetermainNext(NextInput value);
    }

    public abstract class RegexInfo<NextInput, Result, T> : RegexInfo<NextInput, Result>
        where T : RegexStep<NextInput, Result>
    {
        protected T CurrentStep { get; }
        protected RegexInfo<NextInput, Result>? Parent { get; }

        protected RegexInfo(T currentStep, RegexInfo<NextInput, Result>? parent)
            : base()
        {
            CurrentStep = currentStep;
            Parent = parent;
        }
    }
}
