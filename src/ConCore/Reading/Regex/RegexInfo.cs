using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Blocks;

namespace ConCore.Reading.Regex
{
    public abstract class RegexInfo
    {
        /// <summary>
        /// The character position of the first character.
        /// </summary>
        public CharacterPosition Start { get; }
        /// <summary>
        /// The character position of the last character.
        /// </summary>
        public CharacterPosition? End { get; private set; }

        protected RegexInfo(CharacterPosition start)
        {
            Start = start;
        }

        /// <summary>
        /// Set the end position.
        /// </summary>
        /// <param name="end"></param>
        public void ReachedEnd(CharacterPosition end)
        {
            End = end;
        }

        /// <summary>
        /// Determine next step based on the character info.
        /// </summary>
        /// <param name="charInfo">Information about the current character.</param>
        /// <returns></returns>
        public abstract RegexInfo[] DetermainNext(CharInfo charInfo);
    }

    public abstract class RegexInfo<T> : RegexInfo where T : RegexStep
    {
        protected T CurrentStep { get; }
        protected RegexInfo? Parent { get; }

        protected RegexInfo(T currentStep, RegexInfo? parent, CharacterPosition start)
            : base(start)
        {
            CurrentStep = currentStep;
            Parent = parent;
        }
    }
}
