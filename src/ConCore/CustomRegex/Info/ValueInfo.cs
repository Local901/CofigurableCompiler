using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.CustomRegex.Info
{
    public class ValueInfo<Context, Result> : IValueInfo<Context, Result>
    {
        private RegexInfo<Context, Result>? Parent { get; }

        public ValueInfo(Result value, Context context)
            : this(null, value, context) { }
        public ValueInfo(RegexInfo<Context, Result>? parent, Result value, Context context)
            : base(value, context)
        {
            Parent = parent;
        }
        public override IList<IValueInfo<Context, Result>?> DetermainNext(Context value)
        {
            if (Parent == null)
            {
                return [null];
            }
            return Parent.DetermainNext(value);
        }
    }
}
