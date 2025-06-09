using ConCore.Blocks;
using ConCore.CustomRegex.Info;
using ConCore.Key;
using ConCore.Key.Collections;
using ConCore.Key.Components;
using ConCore.Lexing;
using ConCore.Parsing.Simple.Stack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Parsing.Simple
{
    public class EndedBot : IBot
    {
        public Construct? Construct { get; }

        public ILayer? Layer { get; }

        public IValueInfo<bool, Component>? Info => null;

        public ParseStack<IBlock>.StackInterface TokenReference { get; }

        public bool CanEnd => true;

        public EndedBot(ParseStack<IBlock>.StackInterface stackInterface, ILayer? layer, Construct? construct)
        {
            TokenReference = stackInterface;
            Layer = layer;
            Construct = construct;
        }

        public IEnumerable<IBot> DetermainNext(ILanguage language, ParseStack<IBlock> stack, ConstructReferenceCollection referenceCollection, LexResult lexResults)
        {
            yield break;
        }

        public IEnumerable<IBot> DetermainNext(ILanguage language, ConstructReferenceCollection referenceCollection, IReadOnlyList<LexResult> lexResults)
        {
            yield break;
        }

        public IEnumerable<LexOptions> GetLexOptions(ILanguage language)
        {
            yield break;
        }
    }
}
