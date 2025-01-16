using ConCore.CustomRegex;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Test
{
    public class CustomRegex
    {
        private RegexBuilder<bool, int> builder = new RegexBuilder<bool, int>();

        [Test]
        public void ShouldCreateValueStep()
        {
            int value = new Random().Next();
            var step = builder.Value(value);

            var info = step.Start(true);
            Assert.That(info, Has.Count.EqualTo(1));
            Assert.That(info[0].Value, Is.EqualTo(value));
        }

        [Test]
        public void ShouldCreateOrderedStep()
        {
            var step = builder.Ordered(false,
                builder.Value(1),
                builder.Value(2)
            );

            var info = step.Start(true);

            Assert.That(info, Has.Count.EqualTo(1));
            Assert.That(info[0]?.Value, Is.EqualTo(1));

            info = info[0].DetermainNext(true);

            Assert.That(info, Has.Count.EqualTo(1));
            Assert.That(info[0]?.Value, Is.EqualTo(2));

            info = info[0].DetermainNext(true);

            Assert.That(info, Has.Count.EqualTo(1));
            Assert.That(info[0], Is.EqualTo(null));
        }

        [Test]
        public void ShouldCreateAnyStep()
        {
            var step = builder.Any(false,
                builder.Value(1),
                builder.Value(2)
            );

            var info = step.Start(true);

            Assert.That(info, Has.Count.EqualTo(2));
            Assert.That(info.Select((i) => i?.Value), Has.Member(1));
            Assert.That(info.Select((i) => i?.Value), Has.Member(2));

            Assert.That(info[0].DetermainNext(true)[0], Is.EqualTo(null));
            Assert.That(info[1].DetermainNext(true)[0], Is.EqualTo(null));
        }

        [TestCase(5, 0)]
        [TestCase(5, 1)]
        [TestCase(5, 5)]
        public void ShouldCreateRepeatStepWithMaximum(int max, int min)
        {
            var value = new Random().Next();
            var step = builder.Repeat(builder.Value(value), max, min);

            var info = step.Start(true);

            for (int i = 0; i < max; i++)
            {
                Assert.That(info, Has.Count.EqualTo(i >= min ? 2 : 1));
                Assert.That(info.Select(j => j?.Value), Has.Member(value));
                if (i >= min)
                {
                    Assert.That(info.Select(j => j?.Value), Has.Member(null));
                }
                info = info.First((j) => j != null).DetermainNext(true);
            }

            Assert.That(info, Has.Count.EqualTo(1));
            Assert.That(info[0], Is.EqualTo(null));
        }

        [TestCase(5, 5)]
        [TestCase(5, 4)]
        [TestCase(5, 3)]
        [TestCase(5, 2)]
        [TestCase(5, 1)]
        [TestCase(5, 0)]
        public void ShouldCreateSeparatedList(int max, int min)
        {
            var step = builder.SeparatedList(
                builder.Value(1),
                builder.Value(2),
                allowEndingSeparator: SeparatorOptions.ALWAYS,
                maximum: max,
                minimum: min
            );

            var info = step.Start(true);

            for (int i = 0; i < max; i++)
            {
                var values = info.Select(j => j?.Value);
                Assert.That(
                    info,
                    Has.Count.EqualTo((i >= min && i < max)
                        ? 2
                        : 1
                    )
                );
                Assert.That(values, Has.Member(1));
                if (i > min)
                {
                    Assert.That(values, Has.Member(null));
                }

                info = info.Where((j) => j != null)
                    .SelectMany((j) => j.DetermainNext(true))
                    .ToList();

                values = info.Select(j => j?.Value);
                Assert.That(info, Has.Count.EqualTo((i >= min -1 && i < max - 1) ? 2 : 1));
                foreach(var value in values)
                {
                    Assert.That(value, Is.EqualTo(2));
                }

                info = info.Where((j) => j != null)
                    .SelectMany((j) => j.DetermainNext(true))
                    .ToList();
            }

            Assert.That(info, Has.Count.EqualTo(1));
        }
    }
}
