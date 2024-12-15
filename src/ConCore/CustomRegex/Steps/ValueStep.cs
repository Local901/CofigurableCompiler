using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.CustomRegex.Info;
using ConCore.Reading.Regex;

namespace ConCore.CustomRegex.Steps
{
    public class ValueStep<NextInput, Result> : ValidationStep<NextInput, Result>
    {
        private Result result;

        public ValueStep(Result result)
        {
            this.result = result;
        }

        protected override IValueInfo<NextInput, Result> CreateInfo(
            RegexInfo<NextInput, Result>? parent,
            NextInput value
        )
        {
            return new ValueInfo<NextInput, Result>(parent, result);
        }

        protected override bool ValidateValue(NextInput value)
        {
            return true;
        }
    }
}
