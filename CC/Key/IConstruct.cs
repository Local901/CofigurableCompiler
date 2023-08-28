using CC.Key.ComponentTypes;

namespace CC.Key

{
    public abstract class IConstruct : IKey
    {
        public IComponent Component { get; }

        public IConstruct (string key, IComponent component)
        {
            Reference = new KeyLangReference { Key = key };
            Component = component;
        }
    }
}
