using ConDI.Test.TestClasses;
using System.Runtime.InteropServices;

namespace ConDI.Test
{
    public class DependencyInjector
    {
        private Injector injector;

        [SetUp]
        public void Setup()
        {
            injector = new Injector();
            injector.AddScoped<EmptyClass>();
            injector.AddSingleton<DependentClass>();
        }

        [Test]
        public void ShouldCreateClassWithoutParameters()
        {
            var scope = injector.CreateScope();
            var instance = scope.GetInstance<EmptyClass>();
            Assert.IsNotNull(instance);
            Assert.That(instance, Is.InstanceOf<EmptyClass>());
        }

        [Test]
        public void ShouldCreateClassWithParameters()
        {
            var scope = injector.CreateScope();
            var instance = scope.GetInstance<DependentClass>();
            Assert.IsNotNull(instance);
            Assert.That(instance, Is.InstanceOf<DependentClass>());
        }

        [Test]
        public void ShouldCreateSingletonInLowestScope()
        {
            // Two deep scope.
            var scope = injector.CreateScope().CreateScope();

            var dependent = scope.GetInstance<DependentClass>();
            var empty = scope.GetInstance<EmptyClass>();
            Assert.IsNotNull(dependent);
            Assert.IsNotNull(empty);
            Assert.That(dependent.Dependency, Is.Not.Null);
            Assert.That(dependent.Dependency, Is.Not.EqualTo(empty));
        }

        [Test]
        public void ShouldCreateNewInstancesForTransientDependencies()
        {
            Injector injector = new();
            injector.AddTrancient<EmptyClass>();
            var scope = injector.CreateScope();
            var i1 = scope.GetInstance<EmptyClass>();
            var i2 = scope.GetInstance<EmptyClass>();
            Assert.IsNotNull(i1);
            Assert.IsNotNull(i2);
            Assert.False(Object.ReferenceEquals(i1, i2));
        }
    }
}