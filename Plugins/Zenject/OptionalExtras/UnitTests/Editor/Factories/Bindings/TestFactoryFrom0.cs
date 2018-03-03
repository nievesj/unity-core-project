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
    public class TestFactoryFrom0 : ZenjectUnitTestFixture
    {
        [Test]
        public void TestSelf1()
        {
            Container.BindFactory<Foo, Foo.Factory>().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo.Factory>().Create());
        }

        [Test]
        public void TestSelf2()
        {
            Container.BindFactory<Foo, Foo.Factory>().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo.Factory>().Create());
        }

        [Test]
        public void TestSelf3()
        {
            Container.BindFactory<Foo, Foo.Factory>().FromNew().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo.Factory>().Create());
        }

        [Test]
        public void TestConcrete()
        {
            Container.BindFactory<IFoo, IFooFactory>().To<Foo>().NonLazy();

            Assert.IsNotNull(Container.Resolve<IFooFactory>().Create());

            Assert.That(Container.Resolve<IFooFactory>().Create() is Foo);
        }

        interface IFoo
        {
        }

        class IFooFactory : Factory<IFoo>
        {
        }

        class Foo : IFoo
        {
            public class Factory : Factory<Foo>
            {
            }
        }
    }
}
