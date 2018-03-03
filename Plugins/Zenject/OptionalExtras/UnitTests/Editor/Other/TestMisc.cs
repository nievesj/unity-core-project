using System;
using NUnit.Framework;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestMisc : ZenjectUnitTestFixture
    {
        public interface IFoo
        {
        }

        public interface IBar
        {
        }

        public class Foo1 : IFoo, IBar
        {
        }

        public class Foo2 : IFoo, IBar
        {
        }

        [Test]
        public void Test1()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To(typeof(Foo1), typeof(Foo2)).AsSingle();

            var foos = Container.ResolveAll<IFoo>();
            var bars = Container.ResolveAll<IBar>();

            Assert.IsEqual(foos.Count, 2);
            Assert.IsEqual(bars.Count, 2);

            Assert.That(bars.Contains((IBar)foos[0]));
            Assert.That(foos.Contains((IFoo)bars[0]));
        }
    }
}
