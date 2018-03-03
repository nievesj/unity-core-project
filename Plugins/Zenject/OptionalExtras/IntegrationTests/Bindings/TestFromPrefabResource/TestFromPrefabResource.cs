using System;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using ModestTree;
using Assert=ModestTree.Assert;
using Zenject.Tests.Bindings.FromPrefabResource;

namespace Zenject.Tests.Bindings
{
    public class TestFromPrefabResource : ZenjectIntegrationTestFixture
    {
        const string PathPrefix = "TestFromPrefabResource/";

        [UnityTest]
        public IEnumerator TestTransientError()
        {
            PreInstall();
            // Validation should detect that it doesn't exist
            Container.Bind<Foo>().FromComponentInNewPrefabResource(PathPrefix + "asdfasdfas").AsTransient().NonLazy();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestTransient()
        {
            PreInstall();
            Container.Bind<Foo>().FromComponentInNewPrefabResource(PathPrefix + "Foo").AsTransient().NonLazy();
            Container.Bind<Foo>().FromComponentInNewPrefabResource(PathPrefix + "Foo").AsTransient().NonLazy();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(2);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSingle()
        {
            PreInstall();
            Container.Bind<IFoo>().To<Foo>().FromComponentInNewPrefabResource(PathPrefix + "Foo").AsSingle().NonLazy();
            Container.Bind<Foo>().FromComponentInNewPrefabResource(PathPrefix + "Foo").AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSingle2()
        {
            PreInstall();
            // For ToPrefab, the 'AsSingle' applies to the prefab and not the type, so this is valid
            Container.Bind<IFoo>().To<Foo>().FromComponentInNewPrefabResource(PathPrefix + "Foo").AsSingle().NonLazy();
            Container.Bind<Foo>().FromComponentInNewPrefabResource(PathPrefix + "Foo2").AsSingle().NonLazy();
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
            Container.Bind<Foo>().FromComponentInNewPrefabResource(PathPrefix + "Foo").WithGameObjectName("Foo").AsSingle().NonLazy();
            Container.Bind<Bar>().FromComponentInNewPrefabResource(PathPrefix + "Foo").WithGameObjectName("Foo").AsSingle().NonLazy();

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
            Container.Bind(typeof(Foo), typeof(Bar)).FromComponentInNewPrefabResource(PathPrefix + "Foo")
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
            Container.Bind(typeof(Gorp), typeof(Qux)).FromComponentInNewPrefabResource(PathPrefix + "GorpAndQux").AsCached().NonLazy();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestWithArguments()
        {
            PreInstall();
            Container.Bind(typeof(Gorp))
                .FromComponentInNewPrefabResource(PathPrefix + "Gorp").WithGameObjectName("Gorp").AsCached()
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
            Container.Bind<INorf>().FromComponentInNewPrefabResource(PathPrefix + "Norf").AsTransient().NonLazy();

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
            Container.Bind<INorf>().To<Norf>().FromComponentInNewPrefabResource(PathPrefix + "Norf").AsTransient().NonLazy();

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
            Container.Bind(typeof(Jim), typeof(Bob)).FromComponentInNewPrefabResource(PathPrefix + "JimAndBob").AsCached().NonLazy();

            Container.BindInterfacesTo<JimAndBobRunner>().AsSingle().NonLazy();

            PostInstall();
            yield break;
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
