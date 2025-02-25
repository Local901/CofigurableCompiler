using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.CustomRegex.Info
{
    public abstract class IValueInfo<NextInput, Result> : RegexInfo<NextInput, Result>
    {
        /// <summary>
        /// The character position of the first character.
        /// 
        /// Value is null if nothing was checked.
        /// </summary>
        public Result Value { get; }

        public IValueInfo(Result value)
            : base()
        {
            Value = value;
        }
    }
}
