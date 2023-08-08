using CC.Key.ComponentTypes;

namespace CC.Key

{
    public abstract class IConstruct : IKey
    {
        public IComponent Components { get; }

        public IConstruct (string key, IComponent components)
        {
            Reference = new KeyLangReference { Key = key };
            Components = components;
        }
    }
}
