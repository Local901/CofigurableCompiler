using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.CustomRegex.Info;

namespace ConCore.CustomRegex.Steps
{
    public class ValueStep<Context, Result> : ValidationStep<Context, Result>
    {
        private readonly Result result;

        public ValueStep(Result result)
        {
            this.result = result;
        }

        protected override IValueInfo<Context, Result> CreateInfo(
            RegexInfo<Context, Result>? parent,
            Context value
        )
        {
            return new ValueInfo<Context, Result>(parent, result, value);
        }

        protected override bool ValidateValue(Context value)
        {
            return true;
        }
    }
}
