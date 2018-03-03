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
    public class TestFactoryFromIFactoryResolve : ZenjectUnitTestFixture
    {
        [Test]
        public void Test1()
        {
            Container.BindFactory<Foo, Foo.Factory>().FromIFactoryResolve();

            var foo = new Foo();
            Container.BindIFactory<Foo>().FromInstance(foo);

            Assert.IsEqual(Container.Resolve<Foo.Factory>().Create(), foo);
        }

        [Test]
        public void TestIdentifiers()
        {
            Container.BindFactory<Foo, Foo.Factory>().WithId("foo1").FromIFactoryResolve("foo1");
            Container.BindFactory<Foo, Foo.Factory>().WithId("foo2").FromIFactoryResolve("foo2");

            var foo1 = new Foo();
            var foo2 = new Foo();

            Container.BindIFactory<Foo>().WithId("foo1").FromInstance(foo1);
            Container.BindIFactory<Foo>().WithId("foo2").FromInstance(foo2);

            Assert.IsEqual(Container.ResolveId<Foo.Factory>("foo1").Create(), foo1);
            Assert.IsEqual(Container.ResolveId<Foo.Factory>("foo2").Create(), foo2);
        }

        [Test]
        public void TestWithParameters()
        {
            Container.BindFactory<string, int, float, FooWithArgs, FooWithArgs.Factory>().FromIFactoryResolve();

            var foo = new FooWithArgs();
            Container.BindIFactory<string, int, float, FooWithArgs>()
                .FromMethod((d, s, i, f) =>
                    {
                        Assert.IsEqual(s, "asdf");
                        Assert.IsEqual(i, 4);
                        Assert.IsEqual(f, 8.7f);
                        return foo;
                    });

            Assert.IsEqual(Container.Resolve<FooWithArgs.Factory>().Create("asdf", 4, 8.7f), foo);
        }

        public class Foo
        {
            public class Factory : Factory<Foo>
            {
            }
        }

        public class FooWithArgs
        {
            public class Factory : Factory<string, int, float, FooWithArgs>
            {
            }
        }
    }
}



