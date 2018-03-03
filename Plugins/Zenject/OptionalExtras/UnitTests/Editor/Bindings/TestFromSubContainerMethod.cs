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
    public class TestFromSubContainerMethod : ZenjectUnitTestFixture
    {
        [Test]
        public void TestMethodSelfSingle()
        {
            Container.Bind<Foo>().FromSubContainerResolve().ByMethod(InstallFooFacade).AsSingle().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo>().Bar);
        }

        [Test]
        public void TestMethodSelfTransient()
        {
            Container.Bind<Foo>().FromSubContainerResolve().ByMethod(InstallFooFacade).AsTransient().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo>().Bar);
        }

        [Test]
        public void TestDanglingBinding()
        {
            Container.Bind<Bar>().FromSubContainerResolve();

            Assert.Throws(() => Container.Resolve<Bar>());
        }

        [Test]
        public void TestMethodSelfCached()
        {
            Container.Bind<Foo>().FromSubContainerResolve().ByMethod(InstallFooFacade).AsCached().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo>().Bar);
        }

        [Test]
        public void TestMethodSelfSingleMultipleContracts()
        {
            Container.Bind<Foo>().FromSubContainerResolve().ByMethod(InstallFooFacade).AsSingle().NonLazy();
            Container.Bind<Bar>().FromSubContainerResolve().ByMethod(InstallFooFacade).AsSingle().NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>().Bar, Container.Resolve<Bar>());
        }

        [Test]
        public void TestMethodSelfCachedMultipleContracts()
        {
            Container.Bind(typeof(Foo), typeof(Bar)).FromSubContainerResolve().ByMethod(InstallFooFacade).AsCached().NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>().Bar, Container.Resolve<Bar>());
        }

        [Test]
        public void TestMethodConcreteSingle()
        {
            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve().ByMethod(InstallFooFacade).AsSingle().NonLazy();

            Assert.IsNotNull(Container.Resolve<IFoo>().Bar);
        }

        [Test]
        public void TestMethodConcreteTransient()
        {
            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve().ByMethod(InstallFooFacade).AsTransient().NonLazy();

            Assert.IsNotNull(Container.Resolve<IFoo>().Bar);
        }

        [Test]
        public void TestMethodConcreteCached()
        {
            Container.Bind<IFoo>().To<Foo>()
                .FromSubContainerResolve().ByMethod(InstallFooFacade).AsCached().NonLazy();

            Assert.IsNotNull(Container.Resolve<IFoo>().Bar);
        }

        [Test]
        public void TestMethodConcreteSingleMultipleContracts()
        {
            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve().ByMethod(InstallFooFacade).AsSingle().NonLazy();
            Container.Bind<Bar>().FromSubContainerResolve().ByMethod(InstallFooFacade).AsSingle().NonLazy();

            Assert.IsEqual(Container.Resolve<IFoo>().Bar, Container.Resolve<Bar>());
        }

        [Test]
        public void TestMethodConcreteCachedMultipleContracts()
        {
            Container.Bind(typeof(Foo), typeof(IFoo)).To<Foo>().FromSubContainerResolve().ByMethod(InstallFooFacade).AsCached().NonLazy();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestMethodSelfIdentifiersFails()
        {
            Container.Bind<Gorp>().FromSubContainerResolve().ByMethod(InstallFooFacade).AsSingle().NonLazy();

            Assert.Throws(() => Container.Resolve<Gorp>());
        }

        [Test]
        public void TestMethodSelfIdentifiers()
        {
            Container.Bind<Gorp>().FromSubContainerResolve("gorp").ByMethod(InstallFooFacade).AsSingle().NonLazy();

            Assert.IsNotNull(Container.Resolve<Gorp>());
        }

        public class Gorp
        {
        }

        public class Bar
        {
        }

        public interface IFoo
        {
            Bar Bar
            {
                get;
            }
        }

        public class Foo : IFoo
        {
            public Foo(Bar bar)
            {
                Bar = bar;
            }

            public Bar Bar
            {
                get;
                private set;
            }
        }

        void InstallFooFacade(DiContainer container)
        {
            container.Bind<Foo>().AsSingle();
            container.Bind<Bar>().AsSingle();

            container.Bind<Gorp>().WithId("gorp").AsTransient();
        }
    }
}


