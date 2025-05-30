using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Key.Conditions
{
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
        public abstract bool IsMatch(string value);

        /// <summary>
        /// Get a starting point.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="text"></param>
        /// <param name="endPoints"></param>
        /// <returns>The endpoint that matches the first of the endpoints.</returns>
        public abstract ConditionResponse? GetStartingPoint(int startIndex, string text, IReadOnlyList<int> endPoints);

        /// <summary>
        /// Get A number of start points for a text starting from the starting index.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="text"></param>
        /// <returns>List of endpoints.</returns>
        public abstract IEnumerable<ConditionResponse> GetStartPoints(int startIndex, string text);
    }
}
