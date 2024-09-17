using ConCore.Reading.Regex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Reading
{
    public class RegexBuilder
    {
        private static char[][] EscapedValues = new char[][] {
            new char[2] {'\\', '\\'},
            new char[2] {'"', '"'},
            new char[2] {'t', '\t'},
            new char[2] {'n', '\n'},
            new char[2] {'(', '('},
            new char[2] {')', ')'},
            new char[2] {'[', '['},
            new char[2] {']', ']'},
            new char[2] {'{', '{'},
            new char[2] {'}', '}'},
            new char[2] {'^', '^'},
            new char[2] {'$', '$'},
            new char[2] {'*', '*'},
            new char[2] {'+', '+'},
            new char[2] {'!', '!'},
            new char[2] {'?', '?'},
            new char[2] {'-', '-'},
        };

        public RegexBuilder(string pattern)
        {
        }

        public static RegexStep ParsePattern(string pattern)
        {
            int index = 0;
            return Parse(pattern, ref index);
        }

        private static RegexStep Parse(string pattern, ref int index, char exitChar = '\0', bool any = false)
        {
            RegexStep step = any ? new AnyStep(new List<RegexStep>()) : new OrderStep(new List<RegexStep>());

            while (index < pattern.Length)
            {
                char character = pattern[index];

                if (character == exitChar)
                {
                    return step;
                }

                switch (character)
                {
                    case '^':
                        // Start of line
                        step.ChildSteps.Add(new LineStartStep());
                        index++;
                        break;
                    case '$':
                        // End of line
                        step.ChildSteps.Add(new LineEndStep());
                        index++;
                        break;
                    case '?':
                        // Make previous step optional
                        step.ChildSteps[step.ChildSteps.Count - 1].Optional = true;
                        index++;
                        break;
                    case '.':
                        // Any character
                        step.ChildSteps.Add(new AnyCharStep());
                        index++;
                        break;
                    case '*':
                        // Repeat previous step 0-infinite amount
                        var count = step.ChildSteps.Count;
                        if (count == 0)
                        {
                            throw new Exception($"Can't repeat non-existing step. Index {index}");
                        }
                        step.ChildSteps[count - 1] = new RepeatStep(step.ChildSteps[count - 1]);
                        index++;
                        break;
                    case '+':
                        // Repeat previous step 1-infinite amount
                        count = step.ChildSteps.Count;
                        if (count == 0)
                        {
                            throw new Exception($"Can't repeat non-existing step. Index {index}");
                        }
                        step.ChildSteps[count - 1] = new RepeatStep(step.ChildSteps[count - 1], 1);
                        index++;
                        break;
                    case '(':
                        // Group
                        index++;
                        step.ChildSteps.Add(Parse(pattern, ref index, ')'));
                        break;
                    case '[':
                        // Any of
                        index++;
                        step.ChildSteps.Add(Parse(pattern, ref index, ']', true));
                        break;
                    case '{':
                        // Repeat previous step custom amount
                        count = step.ChildSteps.Count;
                        if (count == 0)
                        {
                            throw new Exception($"Can't repeat non-existing step. Index {index}");
                        }
                        int min = 0;
                        int max = 0;
                        index++;
                        character = pattern[index];
                        while (character != ',')
                        {
                            min = min * 10 + int.Parse(character.ToString());
                            index++;
                            character = pattern[index];
                        }
                        index++;
                        character = pattern[index];
                        while (character != '}')
                        {
                            max = max * 10 + int.Parse(character.ToString());
                            index++;
                            character = pattern[index];
                        }
                        index++;
                        break;
                    case '\\':
                        char nextChar = pattern[index + 1];
                        char[]? map;
                        if ((map = EscapedValues.FirstOrDefault((m) => m[0] == nextChar)) != null) {
                            step.ChildSteps.Add(new CharStep(map[1]));
                            index += 2;
                            break;
                        }
                        throw new Exception($"Unknown escape character '{nextChar}' at index {index + 1}");
                    default:
                        step.ChildSteps.Add(new CharStep(character));
                        index++;
                        break;
                }
            }
            if (exitChar == '\0')
            {
                return step;
            }
            throw new Exception();
        }
    }
}
