using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Key.Conditions
{
    public class OrderCondition : ReadCondition
    {
        public readonly IReadOnlyList<ReadCondition> Conditions;

        public OrderCondition(IReadOnlyList<ReadCondition> conditions)
        {
            Conditions = conditions;
        }

        public override bool IsMatch(string value)
        {
            return GetStartingPoint(0, value, [value.Length]) != null;
        }

        public override IEnumerable<ConditionResponse> GetStartPoints(int startIndex, string text)
        {
            if (Conditions.Count == 0)
            {
                return [];
            }

            IReadOnlyList<ConditionResponse> results = [new ConditionResponse(startIndex)];
            foreach (var condition in Conditions)
            {
                results = results.SelectMany((lastResult) => condition.GetStartPoints(lastResult.EndPoint, text)).ToArray();
                if (results.Count == 0)
                {
                    return [];
                }
            }
            return results;
        }

        public override ConditionResponse? GetStartingPoint(int startIndex, string text, IReadOnlyList<int> endPoints)
        {
            var results = GetStartPoints(startIndex, text).ToArray();
            return results.FirstOrDefault((r) => endPoints.Contains(r.EndPoint));
        }
    }
}
