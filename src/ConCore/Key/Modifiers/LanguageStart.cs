using System;

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
            var key = Language.GetKey(Args.KeyReference.Key);
            if (key == null)
            {
                throw new Exception("No starting key found.");
            }
            return key;
        }

        public override KeyLangReference GetKeyReference()
        {
            return Args.KeyReference;
        }
    }
}
