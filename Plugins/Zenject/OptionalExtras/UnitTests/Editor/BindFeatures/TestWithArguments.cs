using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.BindFeatures
{
    [TestFixture]
    public class TestWithArguments : ZenjectUnitTestFixture
    {
        [Test]
        public void Test1()
        {
            Container.Bind<Foo>().AsTransient().WithArguments(3).NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>().Value, 3);
        }

        [Test]
        public void TestSingleSameArgument()
        {
            Container.Bind<IFoo>().To<Foo>().AsSingle().WithArguments(3).NonLazy();
            Container.Bind<Foo>().AsSingle().WithArguments(3).NonLazy();

            Assert.IsNotNull(Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestSingleDifferentArguments()
        {
            Container.Bind<IFoo>().To<Foo>().AsSingle().WithArguments(3);
            Container.Bind<Foo>().AsSingle().WithArguments(2);

            Assert.Throws(() => Container.FlushBindings());
        }

        [Test]
        public void TestNullValues()
        {
            Container.Bind<Foo>().AsSingle().WithArguments(3, (string)null);

            var foo = Container.Resolve<Foo>();

            Assert.IsEqual(foo.Value, 3);
            Assert.IsEqual(foo.Value2, null);
        }

        interface IFoo
        {
        }

        class Foo : IFoo
        {
            public Foo(
                int value,
                [InjectOptional]
                string value2)
            {
                Value = value;
                Value2 = value2;
            }

            public int Value
            {
                get;
                private set;
            }

            public string Value2
            {
                get;
                private set;
            }
        }
    }
}

