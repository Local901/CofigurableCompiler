using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConCore.Reading.Errors;

namespace ConCore.Reading
{
    /// <summary>
    /// Modifier for read characters.
    /// <para>A character can be part of a encoding so these will have to be decoded before considering if they match a token.</para>
    /// <para>When extending/encapsulating this class. Always call the base/child <see cref="TryDecodeCharacter"/>. Because you never know what might change.
    /// <see cref="ReadModifier"/> already implements incapsulation so no need to worry about calling the <see cref="Child"/>,
    /// because the base functionality already andels that for you.</para>
    /// </summary>
    public class ReadModifier
    {
        protected readonly ReadModifier? Child;

        public ReadModifier(ReadModifier? child) {
            Child = child;
        }

        /// <summary>
        /// Try character. If return true a new character has been retured. Else result is not given.
        /// </summary>
        /// <param name="character">The next character to consider.</param>
        /// <param name="result">The resulting character after modification/combination.</param>
        /// <exception cref="UnknownCharacter"></exception>
        /// <returns>True when result is has a character to be considered.</returns>
        public virtual bool TryDecodeCharacter(char character, out char result)
        {
            if (Child != null)
            {
                return Child.TryDecodeCharacter(character, out result);
            }
            result = character;
            return true;
        }

        /// <summary>
        /// Encode a character. to 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public virtual string EncodeCharacter(char character)
        {
            return Child == null ? character.ToString() : Child.EncodeCharacter(character);
        }

        public virtual void Reset() { }
    }
}
