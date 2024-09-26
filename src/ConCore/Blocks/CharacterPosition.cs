using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Blocks
{
    public struct CharacterPosition
    {
        /// <summary>
        /// The global index of the character. (0-based)
        /// </summary>
        public readonly int Index;
        /// <summary>
        /// The index of the character in the line. (0-based)
        /// </summary>
        public readonly int LineIndex;
        /// <summary>
        /// The line number. (0-based)
        /// </summary>
        public readonly int LineNumber;

        public CharacterPosition(int index, int lineIndex, int lineNumber)
        {
            Index = index;
            LineIndex = lineIndex;
            LineNumber = lineNumber;
        }

        /// <summary>
        /// Return a string representation op the character position in a human readable way.
        /// 
        /// * First number is the line starting at 1.
        /// * Second number is the character index in that line starting at 0.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{LineNumber + 1}:{LineIndex}";
        }
    }
}
