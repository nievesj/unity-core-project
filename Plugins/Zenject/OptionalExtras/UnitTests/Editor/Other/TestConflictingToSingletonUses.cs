
using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings.Singletons
{
    [TestFixture]
    public class TestConflictingToSingletonUses : ZenjectUnitTestFixture
    {
        [Test]
        public void TestToSingleMethod1()
        {
            Container.Bind<Foo>().AsSingle();

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromMethod((container) => new Foo()).AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromInstance(new Foo()).AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromFactory<FooFactory>().AsSingle();
                    Container.FlushBindings();
                });
        }

        [Test]
        public void TestToSingleMethod()
        {
            Container.Bind<Foo>().FromMethod((container) => new Foo()).AsSingle();

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromInstance(new Foo()).AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromFactory<FooFactory>().AsSingle();
                    Container.FlushBindings();
                });
        }

        [Test]
        public void TestToSingleInstance()
        {
            Container.Bind<Foo>().FromInstance(new Foo()).AsSingle();

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromMethod((container) => new Foo()).AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromFactory<FooFactory>().AsSingle();
                    Container.FlushBindings();
                });
        }

        [Test]
        public void TestToSingleFactory()
        {
            Container.Bind<Foo>().FromFactory<FooFactory>().AsSingle();

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromMethod((container) => new Foo()).AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromInstance(new Foo()).AsSingle();
                    Container.FlushBindings();
                });
        }

        class Bar
        {
            public Foo GetFoo()
            {
                return new Foo();
            }
        }

        class Foo
        {
        }

        class FooFactory : IFactory<Foo>
        {
            public Foo Create()
            {
                return new Foo();
            }
        }
    }
}


