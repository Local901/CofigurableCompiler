using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Reading.Errors
{
    /// <summary>
    /// Exception thrown when an unknown character is trying to be decoded.
    /// </summary>
    public class UnknownCharacter : ReadException
    {
        public readonly char Character;

        public UnknownCharacter(char character)
            : base($"Unknown character: '{character}'")
        {
            Character = character;
        }
    }
}
