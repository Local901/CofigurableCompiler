using ConCore.Blocks;
using ConCore.Key.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConCore.Key

{
    using Component = ConCore.CustomRegex.Steps.RegexStep<bool, Component>;
    public class Construct : IKey
    {
        public Component Component { get; }

        public Construct (string key, Component component)
            : base(key)
        {
            Component = component;
        }

        public bool Check(IReadOnlyList<IBlock> values)
        {
            var language = Reference.Language;
            var data = Component.DetermainNext(null, true);

            foreach (var value in values)
            {
                // Only use data that corresponds with the possible data.
                data = data.Where((d) => language.IsKeyInGroup(value.Key.Reference, d.Value.Reference)).ToList();

                // If no data remains it is not of this type.
                if (data.Count == 0)
                {
                    return false;
                }

                // Get next data.
                data = data.SelectMany((d) => d.DetermainNext(true)).ToArray();
            }

            return true;
        }
    }
}
