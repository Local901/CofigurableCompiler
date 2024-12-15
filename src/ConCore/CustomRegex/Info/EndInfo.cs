using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Blocks;

namespace ConCore.CustomRegex.Info
{
    internal class EndInfo<NextInput, Result> : IValueInfo<NextInput, Result>
    {
        public override bool EndReached => false;

        public EndInfo()
            : base(default) { }

        public override IValueInfo<NextInput, Result>[] DetermainNext(NextInput value)
        {
            return new IValueInfo<NextInput, Result>[0];
        }
    }
}
