using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using ModestTree;
using UnityEngine.TestTools;
using Assert=ModestTree.Assert;
using Zenject.Tests.Factories.PrefabFactory;

namespace Zenject.Tests.Factories
{
    public class TestPrefabFactory : ZenjectIntegrationTestFixture
    {
        GameObject FooPrefab
        {
            get
            {
                return FixtureUtil.GetPrefab("TestPrefabFactory/Foo");
            }
        }

        [UnityTest]
        public IEnumerator Test1()
        {
            PreInstall();
            Container.Bind<FooFactory>().ToSelf().AsSingle();
            Container.Bind<IInitializable>().To<Runner>().AsSingle().WithArguments(FooPrefab);

            PostInstall();
            yield break;
        }

        public class FooFactory : PrefabFactory<Foo>
        {
        }

        public class Runner : IInitializable
        {
            readonly GameObject _prefab;
            readonly FooFactory _fooFactory;

            public Runner(
                FooFactory fooFactory,
                GameObject prefab)
            {
                _prefab = prefab;
                _fooFactory = fooFactory;
            }

            public void Initialize()
            {
                var foo = _fooFactory.Create(_prefab);

                Assert.That(foo.WasInitialized);
            }
        }
    }
}
