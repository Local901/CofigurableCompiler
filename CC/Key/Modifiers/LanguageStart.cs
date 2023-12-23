using CC.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Key.Modifiers
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
