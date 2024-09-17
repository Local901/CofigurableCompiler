using ConCore.Blocks;
using ConCore.Key;
using ConCore.Reading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConCore.Lexing
{
    public struct TokenArgs
    {
        /// <summary>
        /// Token to parse.
        /// </summary>
        public Token Token;
        /// <summary>
        /// Modifier for modifying the activly read part for matching the token.
        /// </summary>
        public ReadModifier? ReadModifier;
        /// <summary>
        /// Modifier for preceding text. Will only start checking for a match to the token after this fails.
        /// </summary>
        public ReadModifier? PrecendingModifier;
    }

    public struct MatchResult
    {
        public TokenArgs Args;
        public Match Match;
    }

    public struct ReadResponse
    {
        public Token Token;
        public string MatchValue;
        public CharacterPosition StartIndex;
        public CharacterPosition EndIndex;
    }

    public class BufferPart
    {
        public string Value;
        public int Length { get => Value.Length; }
        public CharacterPosition[] LinePositions;

        public BufferPart(string value, CharacterPosition start, CharacterPosition? previous = null)
        {
            CharacterPosition previousPosition = previous ?? new CharacterPosition(0, 0, 0);

            Value = value;
            List<int> newLineIndexes = new List<int>();
            int lastIndex = -1;
            while (true)
            {
                lastIndex = value.IndexOf("\n", lastIndex + 1);
                if (lastIndex == -1)
                {
                    break;
                }
                newLineIndexes.Add(lastIndex);
            }

            var foundNewLines = newLineIndexes.Select((newLineIndex, index) =>
                new CharacterPosition(
                    // start character index + the index of the provided string + 1 to skip the new line (+ 1 to skip the carage return if present)
                    start.Index + newLineIndex + 1 + (newLineIndex + 1 < value.Length && value[newLineIndex + 1] == '\r' ? 1 : 0),
                    // line index always starts at 0
                    0,
                    // line number is provious line number + index in the array + 1 because array index is 0-based
                    start.LineNumber + index + 1)
            );

            if (value[0] == '\r')
            {
                LinePositions = foundNewLines.Prepend(new CharacterPosition(start.Index + 1, 0, start.LineNumber))
                    .Prepend(new CharacterPosition(start.Index, previousPosition.LineIndex + (start.Index - previousPosition.Index), start.LineNumber - 1))
                    .ToArray();
                return;
            }

            LinePositions = foundNewLines.Prepend(start).ToArray();
        }

        public CharacterPosition GetPosition(int index)
        {
            if (index < 0 || index >= Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            var globalIndex = LinePositions[0].Index + index;
            var clossest = LinePositions.Reverse().FirstOrDefault(l => l.Index < globalIndex);
            return new CharacterPosition(
                globalIndex,
                clossest.LineIndex + (globalIndex - clossest.Index),
                clossest.LineNumber
            );
        }
    }

    public class BlockReader
    {
        private readonly StreamReader Stream;
        private bool EndOfFileReached = false;
        private List<BufferPart> Buffer = new List<BufferPart>();
        private int startGlobalIndex = 0;

        public BlockReader(StreamReader stream)
        {
            Stream = stream;
            char[] block = new char[1000];
            var numOfChar = stream.ReadBlock(block, 0, 1000);
            Buffer.Add(new BufferPart(new string(block, 0, numOfChar), new CharacterPosition(0, 0, 0)));
        }

        private CharacterPosition IndexToPosition(int index)
        {
            foreach (var part in Buffer)
            {
                if (index < part.Length)
                {
                    return part.GetPosition(index);
                }
                index -= part.Length;
            }
            throw new IndexOutOfRangeException("Index outside loaded range of characters.");
        }

        public BlockReadResult[] NextBlocks(TokenArgs[] args)
        {
            // Ignore modifiers for now.
            if (!EndOfFileReached && Buffer.Count < 2)
            {
                LoadNextBuffer();
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var block in Buffer)
            {
                stringBuilder.Append(block.Value);
            }

            var bufferPosition = Buffer[0].GetPosition(0);
            int startIndex = startGlobalIndex - bufferPosition.Index;

        searchString: // Start the rearch in the loaded data.
            string stringValue = stringBuilder.ToString();
            var results = FindMatches(stringValue, args, startIndex);
            int minIndex = results.Min((r) => r.Match.Index);
            var first = results.Where((r) => r.Match.Index <= minIndex);

            if (
                (first.Count() == 0 && !EndOfFileReached) ||
                // Check if any of the first matches is connected with the end of the string.
                first.Any((r) => (r.Match.Index + r.Match.Length) == stringValue.Length)
            ) {
                BufferPart? buffer;
                if ((buffer = LoadNextBuffer()) != null)
                {
                    stringBuilder.Append(buffer.Value);
                    goto searchString; // Retry matching with more data.
                }
            }

            string precedingValue = stringValue.Substring(startIndex, startGlobalIndex + minIndex);

            // Update start index;
            startGlobalIndex += minIndex;

            // Remove passed buffer parts
            int globalIndex = bufferPosition.Index + minIndex;
            Buffer.RemoveAll((b) => (b.GetPosition(0).Index + b.Length - 1) < globalIndex);

            // Return results.
            return first.Select((r) => new BlockReadResult
            {
                Key = r.Args.Token,
                MatchValue = r.Match.Value,
                PrecedingValue = precedingValue,
                MatchStart = IndexToPosition(r.Match.Index),
                MatchEnd = IndexToPosition(r.Match.Index + r.Match.Length),
                PrecedingStart = IndexToPosition(startIndex)
            }).ToArray();
        }

        private MatchResult[] FindMatches(string stringValue, TokenArgs[] tokens, int startAt = 0)
        {
            var result = new List<MatchResult>();
            foreach (var token in tokens)
            {
                try
                {
                    var match = token.Token.Regex.Match(stringValue, startAt);
                    result.Add(new MatchResult
                    {
                        Args = token,
                        Match = match
                    });

                } catch(Exception)
                {
                    continue;
                }
            }
            return result.ToArray();
        }

        private BufferPart? LoadNextBuffer()
        {
            char[] block = new char[1000];
            var numOfChar = Stream.ReadBlock(block, 0, 1000);

            if (numOfChar == 0)
            {
                EndOfFileReached = true;
                return null;
            }

            BufferPart lastBuffer = Buffer.Last();
            CharacterPosition previousStart = lastBuffer.LinePositions[0];
            CharacterPosition previousLast = lastBuffer.LinePositions.Last();

            int globalIndex = previousStart.Index + lastBuffer.Length;

            BufferPart bufferPart = new BufferPart(
                new string(block, 0, numOfChar),
                new CharacterPosition(globalIndex, previousLast.LineIndex + (globalIndex - previousLast.Index), previousLast.LineNumber),
                previousLast.Index >= globalIndex ? lastBuffer.LinePositions[lastBuffer.LinePositions.Length - 1] : previousLast
            );

            Buffer.Add(bufferPart);
            return bufferPart;
        }
    }
}
