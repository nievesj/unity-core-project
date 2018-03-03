using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFromGetter : ZenjectUnitTestFixture
    {
        [Test]
        public void TestTransient()
        {
            Container.Bind<Foo>().AsSingle().NonLazy();
            Container.Bind<Bar>().FromResolveGetter<Foo>(x => x.Bar).AsTransient().NonLazy();

            Assert.IsNotNull(Container.Resolve<Bar>());
            Assert.IsEqual(Container.Resolve<Bar>(), Container.Resolve<Foo>().Bar);

            Foo.NumCalls = 0;

            Container.Resolve<Bar>();
            Container.Resolve<Bar>();
            Container.Resolve<Bar>();

            Assert.IsEqual(Foo.NumCalls, 3);
        }

        [Test]
        public void TestCached()
        {
            Container.Bind<Foo>().AsSingle().NonLazy();
            Container.Bind<Bar>().FromResolveGetter<Foo>(x => x.Bar).AsCached().NonLazy();

            Foo.NumCalls = 0;

            Container.Resolve<Bar>();
            Container.Resolve<Bar>();
            Container.Resolve<Bar>();

            Assert.IsEqual(Foo.NumCalls, 1);
        }

        [Test]
        public void TestSingle()
        {
            Container.Bind<Foo>().AsSingle().NonLazy();

            Container.Bind<Bar>().FromResolveGetter<Foo>(BarGetter).AsSingle().NonLazy();

            // Not sure why I need to specify the "<Bar,"
            Container.Bind<IBar>().To<Bar>().FromResolveGetter<Foo>(BarGetter).AsSingle().NonLazy();

            Foo.NumCalls = 0;

            Assert.IsEqual(Container.Resolve<Bar>(), Foo.StaticBar);
            Assert.IsEqual(Container.Resolve<IBar>(), Foo.StaticBar);

            Container.Resolve<Bar>();
            Container.Resolve<Bar>();
            Container.Resolve<IBar>();
            Container.Resolve<IBar>();
            Container.Resolve<Bar>();

            Assert.IsEqual(Foo.NumCalls, 1);
        }

        Bar BarGetter(Foo foo)
        {
            return foo.Bar;
        }

        interface IBar
        {
        }

        class Bar : IBar
        {
        }

        class Foo
        {
            public static int NumCalls = 0;
            public static Bar StaticBar;

            public Foo()
            {
                StaticBar = new Bar();
            }

            public Bar Bar
            {
                get
                {
                    NumCalls++;
                    return StaticBar;
                }
            }
        }
    }
}

