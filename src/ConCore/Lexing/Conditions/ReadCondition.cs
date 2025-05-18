using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Lexing.Conditions
{
    public class ConditionResponse
    {
        public readonly IReadOnlyList<int> Splits;
        public int EndPoint { get => Splits.Last(); }

        public readonly bool CanStartAtAnyPoint;
        public readonly int MinimumStartPoint;
        public readonly int? MaximumStartPoint;

        public ConditionResponse(
            IReadOnlyList<int> splits,
            bool canStartAtAnyPoint = false,
            int minimumStartPoint = 0,
            int? maximumStartPoint = null
        ) {
            Splits = splits;
            CanStartAtAnyPoint = canStartAtAnyPoint;
            MinimumStartPoint = minimumStartPoint;
            MaximumStartPoint = maximumStartPoint;
        }
    }

    /// <summary>
    /// A condition to indecate when the next value is allowed to match.
    /// </summary>
    public abstract class ReadCondition
    {
        /// <summary>
        /// Is the value a match for this condition.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True is the value matches the condition.</returns>
        public virtual bool IsMatch(string value)
        {
            return true;
        }

        /// <summary>
        /// Get a starting point.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="text"></param>
        /// <param name="endPoints"></param>
        /// <returns>The endpoint that matches one of the endpoints.</returns>
        public abstract ConditionResponse? GetStartingPoints(int startIndex, string text, IReadOnlyList<int> endPoints);

        /// <summary>
        /// Get A number of endpoints for a text starting from the starting index.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="text"></param>
        /// <returns>List of endpoints.</returns>
        public abstract IEnumerable<ConditionResponse> GetEndpoints(int startIndex, string text);
    }
}
