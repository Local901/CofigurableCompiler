using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace ConCore.Key.Conditions
{
    public class RegexCondition : ReadCondition
    {
        public readonly Regex Regex;

        public RegexCondition(Regex regex)
        {
            Regex = regex;
        }

        public override bool IsMatch(string value)
        {
            return Regex.Match(value).Value == value;
        }

        public override IEnumerable<ConditionResponse> GetStartPoints(int startIndex, string text)
        {
            var matches = Regex.Matches(text, startIndex)
                .Where((match) => match.Index == startIndex)
                .ToArray();
            if (matches.Length == 0)
            {
                yield break;
            }

            foreach (var match in matches)
            {
                yield return new ConditionResponse(startIndex + match.Length);
            }
        }

        public override ConditionResponse? GetStartingPoint(int startIndex, string text, IReadOnlyList<int> endPoints)
        {
            var responses = GetStartPoints(startIndex, text)
                .Where((response) => endPoints.Contains(response.EndPoint))
                .ToArray();
            
            if (responses.Length == 0)
            {
                return null;
            }

            ConditionResponse response = responses[0];
            for (int i = 1; i < responses.Length; i++)
            {
                if (responses[i].EndPoint > response.EndPoint)
                {
                    response = responses[i];
                }
            }

            return response;
        }
    }
}
