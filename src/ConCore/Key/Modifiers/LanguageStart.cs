namespace ConCore.Key.Modifiers
{
    public struct LanguageStartArgs
    {
        public KeyLangReference KeyReference { get; set; }
    }

    public class LanguageStart : ILanguageFilter
    {
        public readonly LanguageStartArgs Args;

        public LanguageStart(LanguageStartArgs args)
        {
            Args = args;
        }

        public override IKey FindKey()
        {
            return Language.GetKey(Args.KeyReference.Key);
        }
    }
}
