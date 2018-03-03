using System.Collections.Generic;
using NUnit.Framework;
using Assert=ModestTree.Assert;

#pragma warning disable 219

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestMemoryPool0 : ZenjectUnitTestFixture
    {
        [Test]
        public void TestFactoryProperties()
        {
            Container.BindMemoryPool<Foo, Foo.Pool>();

            var factory = Container.Resolve<Foo.Pool>();

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumTotal, 0);
            Assert.IsEqual(factory.NumInactive, 0);

            var foo = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumTotal, 1);
            Assert.IsEqual(factory.NumInactive, 0);
            Assert.IsEqual(foo.ResetCount, 1);

            factory.Despawn(foo);

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumTotal, 1);
            Assert.IsEqual(factory.NumInactive, 1);
            Assert.IsEqual(foo.ResetCount, 1);

            foo = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumTotal, 1);
            Assert.IsEqual(factory.NumInactive, 0);
            Assert.IsEqual(foo.ResetCount, 2);

            var foo2 = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 2);
            Assert.IsEqual(factory.NumTotal, 2);
            Assert.IsEqual(factory.NumInactive, 0);
            Assert.IsEqual(foo2.ResetCount, 1);

            factory.Despawn(foo);

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumTotal, 2);
            Assert.IsEqual(factory.NumInactive, 1);
            Assert.IsEqual(foo.ResetCount, 2);

            factory.Despawn(foo2);

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumTotal, 2);
            Assert.IsEqual(factory.NumInactive, 2);
        }

        [Test]
        public void TestFactoryPropertiesDefault()
        {
            Container.BindMemoryPool<Foo>();

            var factory = Container.Resolve<MemoryPool<Foo>>();

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumTotal, 0);
            Assert.IsEqual(factory.NumInactive, 0);

            var foo = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumTotal, 1);
            Assert.IsEqual(factory.NumInactive, 0);

            factory.Despawn(foo);

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumTotal, 1);
            Assert.IsEqual(factory.NumInactive, 1);

            foo = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumTotal, 1);
            Assert.IsEqual(factory.NumInactive, 0);

            var foo2 = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 2);
            Assert.IsEqual(factory.NumTotal, 2);
            Assert.IsEqual(factory.NumInactive, 0);

            factory.Despawn(foo);

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumTotal, 2);
            Assert.IsEqual(factory.NumInactive, 1);

            factory.Despawn(foo2);

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumTotal, 2);
            Assert.IsEqual(factory.NumInactive, 2);
        }

        [Test]
        public void TestExpandDouble()
        {
            Container.BindMemoryPool<Foo, Foo.Pool>().ExpandByDoubling();

            var factory = Container.Resolve<Foo.Pool>();

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumTotal, 0);
            Assert.IsEqual(factory.NumInactive, 0);

            var foo = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumTotal, 1);
            Assert.IsEqual(factory.NumInactive, 0);

            var foo2 = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 2);
            Assert.IsEqual(factory.NumTotal, 2);
            Assert.IsEqual(factory.NumInactive, 0);

            var foo3 = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 3);
            Assert.IsEqual(factory.NumTotal, 4);
            Assert.IsEqual(factory.NumInactive, 1);

            factory.Despawn(foo2);

            Assert.IsEqual(factory.NumActive, 2);
            Assert.IsEqual(factory.NumTotal, 4);
            Assert.IsEqual(factory.NumInactive, 2);

            var foo4 = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 3);
            Assert.IsEqual(factory.NumTotal, 4);
            Assert.IsEqual(factory.NumInactive, 1);
        }

        [Test]
        public void TestFixedSize()
        {
            Container.BindMemoryPool<Foo, Foo.Pool>().WithFixedSize(2);

            var factory = Container.Resolve<Foo.Pool>();

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumTotal, 2);
            Assert.IsEqual(factory.NumInactive, 2);

            var foo = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumTotal, 2);
            Assert.IsEqual(factory.NumInactive, 1);

            var foo2 = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 2);
            Assert.IsEqual(factory.NumTotal, 2);
            Assert.IsEqual(factory.NumInactive, 0);

            Assert.Throws<PoolExceededFixedSizeException>(() => factory.Spawn());
        }

        [Test]
        public void TestInitialSize()
        {
            Container.BindMemoryPool<Foo, Foo.Pool>().WithInitialSize(5);

            var factory = Container.Resolve<Foo.Pool>();

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumTotal, 5);
            Assert.IsEqual(factory.NumInactive, 5);
        }

        [Test]
        public void TestExpandManually()
        {
            Container.BindMemoryPool<Foo, Foo.Pool>();

            var factory = Container.Resolve<Foo.Pool>();

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumTotal, 0);
            Assert.IsEqual(factory.NumInactive, 0);

            factory.ExpandPoolBy(2);

            Assert.IsEqual(factory.NumActive, 0);
            Assert.IsEqual(factory.NumTotal, 2);
            Assert.IsEqual(factory.NumInactive, 2);

            var foo = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumTotal, 2);
            Assert.IsEqual(factory.NumInactive, 1);

            factory.ExpandPoolBy(3);

            Assert.IsEqual(factory.NumActive, 1);
            Assert.IsEqual(factory.NumTotal, 5);
            Assert.IsEqual(factory.NumInactive, 4);

            var foo2 = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 2);
            Assert.IsEqual(factory.NumTotal, 5);
            Assert.IsEqual(factory.NumInactive, 3);

            var foo3 = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 3);
            Assert.IsEqual(factory.NumTotal, 5);
            Assert.IsEqual(factory.NumInactive, 2);

            factory.ExpandPoolBy(1);

            Assert.IsEqual(factory.NumActive, 3);
            Assert.IsEqual(factory.NumTotal, 6);
            Assert.IsEqual(factory.NumInactive, 3);

            factory.Despawn(foo2);

            Assert.IsEqual(factory.NumActive, 2);
            Assert.IsEqual(factory.NumTotal, 6);
            Assert.IsEqual(factory.NumInactive, 4);

            var foo4 = factory.Spawn();

            Assert.IsEqual(factory.NumActive, 3);
            Assert.IsEqual(factory.NumTotal, 6);
            Assert.IsEqual(factory.NumInactive, 3);
        }

        class Bar
        {
            public Bar()
            {
            }

            public class Pool : MemoryPool<Bar>
            {
            }
        }

        class Foo
        {
            public Foo()
            {
            }

            public int ResetCount
            {
                get; private set;
            }

            public class Pool : MemoryPool<Foo>
            {
                protected override void OnSpawned(Foo foo)
                {
                    foo.ResetCount++;
                }
            }
        }

        [Test]
        public void TestSubContainers()
        {
            Container.BindMemoryPool<Qux, Qux.Pool>()
                .FromSubContainerResolve().ByMethod(InstallQux).NonLazy();

            var factory = Container.Resolve<Qux.Pool>();
            var qux = factory.Spawn();
        }

        void InstallQux(DiContainer subContainer)
        {
            subContainer.Bind<Qux>().AsSingle();
        }

        class Qux
        {
            public class Pool : MemoryPool<Qux>
            {
            }
        }

        [Test]
        [ValidateOnly]
        public void TestIds()
        {
            Container.BindMemoryPool<Foo, Foo.Pool>().WithInitialSize(5).WithId("foo");

            var pool = Container.ResolveId<Foo.Pool>("foo");
        }
    }
}

