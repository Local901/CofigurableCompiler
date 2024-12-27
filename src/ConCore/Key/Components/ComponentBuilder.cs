using ConCore.CustomRegex;
using ConCore.CustomRegex.Steps;

namespace ConCore.Key.Components
{
    public class ComponentBuilder : RegexBuilder<bool, Component>
    {
        public RegexStep<bool, Component> Value(KeyLangReference reference, string? name = null)
        {
            return Value(new Component(reference, name));
        }
    }
}
