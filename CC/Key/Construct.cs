using CC.Key.ComponentTypes;

namespace CC.Key

{
    public class Construct : IKey
    {
        public IComponent Component { get; }

        public Construct (string key, IComponent component)
        {
            Reference = new KeyLangReference { Key = key };
            Component = component;
        }
    }
}
