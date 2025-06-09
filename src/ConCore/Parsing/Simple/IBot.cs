using ConCore.Blocks;
using ConCore.CustomRegex.Info;
using ConCore.Key.Collections;
using ConCore.Key;
using ConCore.Lexing;
using ConCore.Parsing.Simple.Stack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Key.Components;

namespace ConCore.Parsing.Simple
{
    public interface IBot
    {
        Construct? Construct { get; }
        ILayer? Layer { get; }
        IValueInfo<bool, Component>? Info { get; }
        ParseStack<IBlock>.StackInterface TokenReference { get; }
        IEnumerable<IBot> DetermainNext(ILanguage language, ParseStack<IBlock> stack, ConstructReferenceCollection referenceCollection, LexResult lexResults);
        IEnumerable<LexOptions> GetLexOptions(ILanguage language);
    }
}
