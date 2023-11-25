using CC.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Key.Modifiers
{
    public interface LanguageStartArgs
    {
        string KeyReference { get; set; }
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
            return Language.GetKey(Args.KeyReference);
        }
    }
}
