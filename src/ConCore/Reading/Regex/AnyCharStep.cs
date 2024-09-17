using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Reading.Regex
{
    public class AnyCharStep : CharStep
    {
        public AnyCharStep()
            : base('a') { }

        protected override bool ValidateCharacter(CharInfo charInfo)
        {
            return true;
        }
    }
}
