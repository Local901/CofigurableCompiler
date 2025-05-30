using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Key.Conditions
{
    public class OrCondition : ReadCondition
    {
        public readonly IReadOnlyList<ReadCondition> Conditions;

        public OrCondition(IReadOnlyList<ReadCondition> conditions)
        {
            Conditions = conditions;
        }

        public override bool IsMatch(string value)
        {
            return Conditions.Any((condition) => condition.IsMatch(value));
        }

        public override IEnumerable<ConditionResponse> GetStartPoints(int startIndex, string text)
        {
            return Conditions.SelectMany((condition) => condition.GetStartPoints(startIndex, text));
        }

        public override ConditionResponse? GetStartingPoint(int startIndex, string text, IReadOnlyList<int> endPoints)
        {
            foreach (var condition in Conditions)
            {
                var response = condition.GetStartingPoint(startIndex, text, endPoints);
                if (response != null)
                {
                    return response;
                }
            }
            return null;
        }
    }
}
