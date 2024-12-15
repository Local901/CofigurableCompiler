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
        public override IValueInfo<NextInput, Result>[] DetermainNext(NextInput value)
        {
            if (Parent == null)
            {
                return new IValueInfo<NextInput, Result>[] { new EndInfo<NextInput, Result>() };
            }
            return Parent.DetermainNext(value);
        }
    }
}
