using System;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using ModestTree;
using Assert=ModestTree.Assert;
using Zenject.Tests.Bindings.FromPrefab;

namespace Zenject.Tests.Bindings
{
    public class TestLazy : ZenjectIntegrationTestFixture
    {
        [UnityTest]
        public IEnumerator Test1()
        {
            PreInstall();
            Bar.InstanceCount = 0;

            Container.Bind<Bar>().AsSingle();
            Container.Bind<Foo>().AsSingle();

            PostInstall();

            var foo = Container.Resolve<Foo>();

            Assert.IsEqual(Bar.InstanceCount, 0);

            foo.DoIt();

            Assert.IsEqual(Bar.InstanceCount, 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator Test2()
        {
            PreInstall();
            Container.Bind<Foo>().AsSingle().NonLazy();

            PostInstall();

            var foo = Container.Resolve<Foo>();
            Assert.Throws(() => foo.DoIt());
            yield break;
        }

        [UnityTest]
        [ValidateOnly]
        public IEnumerator Test3()
        {
            PreInstall();
            Container.Bind<Foo>().AsSingle().NonLazy();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        [ValidateOnly]
        public IEnumerator Test4()
        {
            PreInstall();
            Container.Bind<Foo>().AsSingle().NonLazy();
            Container.Bind<Bar>().AsSingle();
            PostInstall();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestOptional1()
        {
            PreInstall();
            Container.Bind<Bar>().AsSingle();
            Container.Bind<Qux>().AsSingle();
            PostInstall();

            Assert.IsNotNull(Container.Resolve<Qux>().Bar.Value);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestOptional2()
        {
            PreInstall();
            Container.Bind<Qux>().AsSingle();
            PostInstall();

            Assert.IsNull(Container.Resolve<Qux>().Bar.Value);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestOptional3()
        {
            PreInstall();
            Container.Bind<Gorp>().AsSingle();
            PostInstall();

            var gorp = Container.Resolve<Gorp>();
            object temp;
            Assert.Throws(() => temp = gorp.Bar.Value);
            yield break;
        }

        public class Bar
        {
            public static int InstanceCount = 0;

            public Bar()
            {
                InstanceCount++;
            }

            public void DoIt()
            {
            }
        }

        public class Foo
        {
            readonly Lazy<Bar> _bar;

            public Foo(Lazy<Bar> bar)
            {
                _bar = bar;
            }

            public void DoIt()
            {
                _bar.Value.DoIt();
            }
        }

        public class Qux
        {
            [Inject(Optional = true)]
            public Lazy<Bar> Bar;
        }

        public class Gorp
        {
            public Lazy<Bar> Bar;
        }
    }
}
