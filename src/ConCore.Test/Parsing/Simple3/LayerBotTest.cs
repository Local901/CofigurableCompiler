using ConCore.Key.Collections;
using ConCore.Key.Components;
using ConCore.Key;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Blocks;
using ConCore.Parsing.Simple.Stack;
using ConCore.Parsing.Simple;
using ConCore.CustomRegex.Info;
using ConCore.Lexing;

namespace ConCore.Test.Parsing.Simple3
{
    public class LayerBotTest
    {
        private Language language;

        [SetUp]
        public void Setup()
        {
            language = new Language("test_lang");

            var startT = language.AddKey(new Token("start", "start"));
            var endT = language.AddKey(new Token("end", "end"));

            var builder = new ComponentBuilder();

            var lineC = language.AddKey(new Construct("line",
                builder.Ordered(false,
                    builder.Value(startT),
                    builder.Value(endT)
                )
            ));

            language.StartingKeyReference = lineC;
        }
    }
}
