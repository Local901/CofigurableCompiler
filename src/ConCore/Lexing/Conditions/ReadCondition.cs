using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Lexing.Conditions
{
    public enum ReadConditionType
    {
        /// <summary>
        /// The token has to start emediatly at the found start index
        /// </summary>
        STRICT,
        /// <summary>
        /// The token has to apear somewear at or after the found start index.
        /// </summary>
        PRECEDING,
    }

    public abstract class ReadCondition
    {
        public readonly ReadConditionType Type;

        public ReadCondition(ReadConditionType type)
        {
            Type = type;
        }

        /// <summary>
        /// Find the index that a token may start on for a given text.
        /// </summary>
        /// <param name="text">The text for which to find the start index.</param>
        /// <param name="startIndex">The index from which to start checking.</param>
        /// <returns>The start index the token may start on. If -1 no start index found.</returns>
        public abstract int StartIndex(string text, int startIndex = 0);
        /// <summary>
        /// Find all indeces that a token may start on for a given text.
        /// </summary>
        /// <param name="text">The text for which to find the start indeces.</param>
        /// <param name="startIndex">The index from which to start checking.</param>
        /// <returns>A array of indeces that a token may start on. This aaray can be empty mhen no starting points are found</returns>
        public abstract int[] AllStartIndeces(string text, int startIndex = 0);
    }
}
