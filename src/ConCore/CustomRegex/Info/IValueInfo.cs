using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.CustomRegex.Info
{
    public abstract class IValueInfo<CONTEXT, RESULT> : RegexInfo<CONTEXT, RESULT>
    {
        /// <summary>
        /// The character position of the first character.
        /// 
        /// Value is null if nothing was checked.
        /// </summary>
        public RESULT Value { get; }
        /// <summary>
        /// Step context recieved from previous.
        /// </summary>
        public CONTEXT Context { get; }

        public IValueInfo(RESULT value, CONTEXT context)
            : base()
        {
            Value = value;
            Context = context;
        }
    }
}
