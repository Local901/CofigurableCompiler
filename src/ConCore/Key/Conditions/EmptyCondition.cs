using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Key.Conditions
{
    public class EmptyCondition : ReadCondition
    {
        public override IEnumerable<ConditionResponse> GetStartPoints(int startIndex, string text)
        {
            yield return new ConditionResponse(startIndex);
        }

        public override ConditionResponse? GetStartingPoint(int startIndex, string text, IReadOnlyList<int> endPoints)
        {
            if (!endPoints.Contains(startIndex))
            {
                return null;
            }
            return new ConditionResponse(startIndex);
        }

        public override bool IsMatch(string value)
        {
            return true;
        }
    }
}
