using ConCore.Key.Conditions;

namespace ConCore.Key.Components
{
    public class PrecedingOptions
    {
        public readonly TemplateCondition TemplateCondition;
        public readonly bool ShouldCapture;
        public readonly string? Name;

        public PrecedingOptions(TemplateCondition templateCondition, bool shouldCapture, string? name)
        {
            TemplateCondition = templateCondition;
            ShouldCapture = shouldCapture;
            Name = name;
        }
    }

    public class Component
    {
        public KeyLangReference Reference { get; }
        public string? Name { get; }
        public PrecedingOptions? PrecedingOptions { get; }

        public Component(KeyLangReference key, string? name = null)
        {
            Reference = key;
            Name = name;
            PrecedingOptions = null;
        }
        public Component(KeyLangReference key, PrecedingOptions precedingOptions, string? name = null)
            : this(key, name) {
            PrecedingOptions = precedingOptions;
        }
    }
}
