using ConCore.Key;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConCore.Parsing.Exceptions
{
    public class ExpectedNextParseException : ParseException
    {
        public readonly IReadOnlyList<KeyLangReference> ExpectedReferences;

        public ExpectedNextParseException(IEnumerable<KeyLangReference> expectedReferences)
            : base(
                  expectedReferences.Select((r) => r.ToString())
                    .Prepend("Expected one of next values ut got none:")
                    .Aggregate((s1, s2) => $"{s1}{Environment.NewLine}  * {s2}"),
                  null,
                  ParseExceptionType.ExpectedNext
              )
        {
            ExpectedReferences = expectedReferences.ToList();
        }
    }
}
