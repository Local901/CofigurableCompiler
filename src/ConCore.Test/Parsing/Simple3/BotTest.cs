using ConCore.Blocks;
using ConCore.Key;
using ConCore.Key.Collections;
using ConCore.Key.Components;
using ConCore.Lexing;
using ConCore.Parsing.Simple;
using ConCore.Parsing.Simple.Stack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Test.Parsing.Simple3
{
    public class BotTest
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
        public void ShouldDetermainNext()
        {
            // The senario of having a bot without a construct and layer having a series of expected tokens is not realistic but good to test.

            var stack = new LinkedParseStack<IBlock>();
            var bot = new Bot(
                stack.GetRoot(),
                (language.GetKey(language.StartingKeyReference) as Construct)?.Component.Start(false)[0]
            );
            var result = bot.DetermainNext(
                language,
                stack,
                new LexResult()
                {
                    Block = new Block(
                        language.GetKey(new KeyLangReference(language, "start")),
                        "start",
                        new CharacterPosition(0, 0, 0),
                        new CharacterPosition(0, 0, 0)
                    )
                }
            ).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Info.Value.Reference, Is.EqualTo(new KeyLangReference(language, "end")));
        }

        [Test]
        public void ShouldEndInEndBot()
        {
            // The senario of having a bot without a construct and layer having a series of expected tokens is not realistic but good to test.

            var stack = new LinkedParseStack<IBlock>();
            var bot = new Bot(
                stack.GetRoot(),
                (language.GetKey(language.StartingKeyReference) as Construct)?.Component.Start(false)[0]
            );
            var result = bot.DetermainNext(
                language,
                stack,
                new LexResult()
                {
                    Block = new Block(
                        language.GetKey(new KeyLangReference(language, "start")),
                        "start",
                        new CharacterPosition(0, 0, 0),
                        new CharacterPosition(0, 0, 0)
                    )
                }
            ).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Info.Value.Reference, Is.EqualTo(new KeyLangReference(language, "end")));

            result = result[0].DetermainNext(
                language,
                stack,
                new LexResult()
                {
                    Block = new Block(
                        language.GetKey(new KeyLangReference(language, "end")),
                        "end",
                        new CharacterPosition(0, 0, 0),
                        new CharacterPosition(0, 0, 0)
                    )
                }
            ).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0], Is.InstanceOf<EndedBot>());
        }
    }
}
