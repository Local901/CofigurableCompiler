using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.CustomRegex.Info
{
    public class ValueInfo<NextInput, Result> : IValueInfo<NextInput, Result>
    {
        private RegexInfo<NextInput, Result>? Parent { get; }

        public ValueInfo(Result value)
            : this(null, value) { }
        public ValueInfo(RegexInfo<NextInput, Result>? parent, Result value)
            : base(value)
        {
            Parent = parent;
        }
        public override IList<IValueInfo<NextInput, Result>?> DetermainNext(NextInput value)
        {
            if (Parent == null)
            {
                return new IValueInfo<NextInput, Result>?[] { null };
            }
            return Parent.DetermainNext(value);
        }
    }
}
