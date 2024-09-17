using ConCore.Blocks;
using ConCore.Key;
using ConCore.Reading.Errors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Reading
{
    public struct ReadResult
    {
        /// <summary>
        /// The token that the matching value is matched with.
        /// </summary>
        public readonly Token token;
        /// <summary>
        /// The index of first character of the matching value.
        /// </summary>
        public readonly CharacterPosition index;
        /// <summary>
        /// The index one after the last character of the matching value.
        /// </summary>
        public readonly CharacterPosition endIndex;
        /// <summary>
        /// The matching string value.
        /// </summary>
        public readonly string match;
        /// <summary>
        /// Decoded matching string value.
        /// </summary>
        public readonly string decodedMatch;
        /// <summary>
        /// The string value between the previous index and the start of the matched string.
        /// </summary>
        public readonly string between;
        /// <summary>
        /// The decoded string value betwen the last index and the start of the matched string.
        /// </summary>
        public readonly string decodedBetween;
    }

    public struct ReadRequestArg
    {
        public readonly KeyLangReference key;
        /// <summary>
        /// Modifier aplied to the characters that are used for the regex.
        /// </summary>
        public readonly ReadModifier? modifier;
        // TODO: Add custom regex implementation.
    }

    public class SimpleStreamReader
    {
        private readonly StreamReader _stream;
        private CharacterPosition CurrentPosition;

        // Saved characters
        private string alreadyReadString = "";
        private int readingIndex = 0;

        public SimpleStreamReader(StreamReader stream)
        {
            _stream = stream;
        }

        public ReadResult ReadNext(IReadOnlyList<ReadRequestArg> args)
        {
            readingIndex = 0;
            var consideredRegs = new List<ActiveRegs>();
            StringBuilder readCharacters = new StringBuilder();

            throw new NotImplementedException();
        }

        private ReadResult ReadOne(ReadRequestArg arg)
        {
            readingIndex = 0;
            StringBuilder readCharacters = new StringBuilder();
            StringBuilder decodedReadCharacters = new StringBuilder();

            throw new NotImplementedException();
            // Regex on decoded string.
        }

        private char NextChar()
        {
            if (readingIndex >= alreadyReadString.Length)
            {
                return (char)_stream.Read();
            }
            return alreadyReadString[readingIndex++];
        }

        private struct ActiveRegs
        {
            public readonly ReadRequestArg arg;
            public readonly CharacterPosition index;
        }
    }
}
