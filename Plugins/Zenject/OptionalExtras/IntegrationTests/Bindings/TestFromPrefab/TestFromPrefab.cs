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
    public class TestFromPrefab : ZenjectIntegrationTestFixture
    {
        GameObject FooPrefab
        {
            get { return GetPrefab("Foo"); }
        }

        GameObject FooPrefab2
        {
            get { return GetPrefab("Foo2"); }
        }

        GameObject GorpPrefab
        {
            get { return GetPrefab("Gorp"); }
        }

        GameObject GorpAndQuxPrefab
        {
            get { return GetPrefab("GorpAndQux"); }
        }

        GameObject NorfPrefab
        {
            get { return GetPrefab("Norf"); }
        }

        GameObject JimAndBobPrefab
        {
            get { return GetPrefab("JimAndBob"); }
        }

        [UnityTest]
        public IEnumerator TestTransient()
        {
            PreInstall();
            Container.Bind<Foo>().FromComponentInNewPrefab(FooPrefab).AsTransient().NonLazy();
            Container.Bind<Foo>().FromComponentInNewPrefab(FooPrefab).AsTransient().NonLazy();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(2);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSingle()
        {
            PreInstall();
            Container.Bind<IFoo>().To<Foo>().FromComponentInNewPrefab(FooPrefab).AsSingle().NonLazy();
            Container.Bind<Foo>().FromComponentInNewPrefab(FooPrefab).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSingle2()
        {
            PreInstall();
            // For ToPrefab, the 'AsSingle' applies to the prefab and not the type, so this is valid
            Container.Bind<IFoo>().To<Foo>().FromComponentInNewPrefab(FooPrefab).AsSingle().NonLazy();
            Container.Bind<Foo>().FromComponentInNewPrefab(FooPrefab2).AsSingle().NonLazy();
            Container.Bind<Foo>().FromMethod(ctx => ctx.Container.CreateEmptyGameObject("Foo").AddComponent<Foo>()).NonLazy();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(3);
            FixtureUtil.AssertNumGameObjects(3);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSingleIdentifiers()
        {
            PreInstall();
            Container.Bind<Foo>().FromComponentInNewPrefab(FooPrefab).WithGameObjectName("Foo").AsSingle().NonLazy();
            Container.Bind<Bar>().FromComponentInNewPrefab(FooPrefab).WithGameObjectName("Foo").AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertComponentCount<Bar>(1);
            FixtureUtil.AssertNumGameObjectsWithName("Foo", 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestCached1()
        {
            PreInstall();
            Container.Bind(typeof(Foo), typeof(Bar)).FromComponentInNewPrefab(FooPrefab)
                .WithGameObjectName("Foo").AsCached().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertComponentCount<Bar>(1);
            FixtureUtil.AssertNumGameObjectsWithName("Foo", 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestWithArgumentsFail()
        {
            PreInstall();
            // They have required arguments
            Container.Bind(typeof(Gorp), typeof(Qux)).FromComponentInNewPrefab(GorpAndQuxPrefab).AsCached().NonLazy();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestWithArgumentsFail2()
        {
            PreInstall();
            Container.Bind(typeof(Gorp), typeof(Qux))
                .FromComponentInNewPrefab(GorpAndQuxPrefab).WithGameObjectName("Gorp").AsCached()
                .WithArguments(5, "test1").NonLazy();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestWithArgumentsSuccess()
        {
            PreInstall();
            Container.Bind<Gorp>().FromComponentInNewPrefab(GorpPrefab)
                .WithGameObjectName("Gorp").AsCached()
                .WithArguments("test1").NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Gorp>(1);
            FixtureUtil.AssertNumGameObjectsWithName("Gorp", 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestWithAbstractSearch()
        {
            PreInstall();
            // There are three components that implement INorf on this prefab
            // and so this should result in a list of 3 INorf's
            Container.Bind<INorf>().FromComponentInNewPrefab(NorfPrefab).AsTransient().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<INorf>(3);
            FixtureUtil.AssertResolveCount<INorf>(Container, 3);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestAbstractBindingConcreteSearch()
        {
            PreInstall();
            // Should ignore the Norf2 component on it
            Container.Bind<INorf>().To<Norf>().FromComponentInNewPrefab(NorfPrefab).AsTransient().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertResolveCount<INorf>(Container, 2);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestCircularDependencies()
        {
            PreInstall();
            // Jim and Bob both depend on each other
            Container.Bind(typeof(Jim), typeof(Bob)).FromComponentInNewPrefab(JimAndBobPrefab).AsCached().NonLazy();
            Container.BindInterfacesTo<JimAndBobRunner>().AsSingle().NonLazy();

            PostInstall();
            yield break;
        }

        GameObject GetPrefab(string name)
        {
            return FixtureUtil.GetPrefab("TestFromPrefab/{0}".Fmt(name));
        }

        public class JimAndBobRunner : IInitializable
        {
            readonly Bob _bob;
            readonly Jim _jim;

            public JimAndBobRunner(Jim jim, Bob bob)
            {
                _bob = bob;
                _jim = jim;
            }

            public void Initialize()
            {
                Assert.IsNotNull(_jim.Bob);
                Assert.IsNotNull(_bob.Jim);

                ModestTree.Log.Info("Jim and bob successfully got the other reference");
            }
        }
    }
}
