namespace ConCore.Key.Components
{
    public class Component
    {
        public KeyLangReference Reference { get; }
        public string? Name { get; }

        public Component(KeyLangReference key) 
            : this(key, null) { }
        public Component(KeyLangReference key, string? name)
        {
            Reference = key;
            Name = name;
        }
    }
}
