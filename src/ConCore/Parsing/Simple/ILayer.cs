using ConCore.Blocks;
using ConCore.CustomRegex.Info;
using ConCore.Key;
using ConCore.Key.Collections;
using ConCore.Key.Components;
using ConCore.Parsing.Simple.Stack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Parsing.Simple
{
    public interface ILayer
    {
        Construct? Construct { get; }
        ILayer? Layer { get; }
        IValueInfo<bool, Component>? Info { get; }
        ParseStack<IBlock>.StackInterface TokenReference { get; }

        IEnumerable<IBot> GetBots(ILanguage language, ParseStack<IBlock>.StackInterface stackInterface);
        IEnumerable<IBot> DetermainNext(ILanguage language, ParseStack<IBlock> stack, IBot childBot);
    }
}
