using ConCore.Key.Collections;
using ConCore.Key.Components;
using ConCore.Key;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConCore.Blocks;
using ConCore.Parsing.Simple3.Stack;
using ConCore.Parsing.Simple3;
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

        [Test]
        public void ShouldAcceptLexResult()
        {
            var stack = new LinkedParseStack<IBlock>();
            var layer = new LayerInstance(
                stack.GetRoot(),
                new ValueInfo<bool, Component>(new Component(language.StartingKeyReference)),
                language
            );

            var result = layer.DetermainNext(language, stack, new LexResult[]
            {
                new LexResult()
                {
                    Block = new Block(
                        language.GetKey(new KeyLangReference(language, "start")),
                        "start",
                        new CharacterPosition(0, 0, 0),
                        new CharacterPosition(4, 4, 0)
                    )
                }
            }).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Info.Value.Reference, Is.EqualTo(new KeyLangReference(language, "end")));
        }
    }
}
