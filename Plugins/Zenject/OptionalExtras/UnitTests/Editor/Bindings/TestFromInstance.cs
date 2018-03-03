using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFromInstance : ZenjectUnitTestFixture
    {
        [Test]
        public void TestTransient()
        {
            var foo = new Foo();

            Container.Bind<IFoo>().FromInstance(foo).NonLazy();
            Container.Bind<Foo>().FromInstance(foo).NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<Foo>(), foo);
        }

        [Test]
        public void TestSingle()
        {
            Container.Bind<Foo>().FromInstance(new Foo()).AsSingle().NonLazy();
            Container.Bind<Foo>().AsSingle().NonLazy();

            Assert.Throws(() => Container.FlushBindings());
        }

        // There's really no good reason to do this but it is part of the api
        [Test]
        public void TestCached()
        {
            var foo = new Foo();

            Container.Bind<IFoo>().FromInstance(foo).AsCached().NonLazy();
            Container.Bind<Foo>().FromInstance(foo).AsCached().NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<Foo>(), foo);
        }

        interface IFoo
        {
        }

        class Foo : IFoo
        {
        }
    }
}

