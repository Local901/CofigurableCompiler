using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Blocks;

namespace ConCore.Reading.Regex
{
    public struct CharInfo
    {
        /// <summary>
        /// Position of the current character.
        /// </summary>
        public CharacterPosition CurrentPosition;
        /// <summary>
        /// The current character.
        /// </summary>
        public char CurrentChar;
        /// <summary>
        /// Position of the previous character.
        /// </summary>
        public CharacterPosition PreviousPosition;
        /// <summary>
        /// The previous character. (is '\0' when at the start of the string.)
        /// </summary>
        public char PreviousChar;
    }
}
