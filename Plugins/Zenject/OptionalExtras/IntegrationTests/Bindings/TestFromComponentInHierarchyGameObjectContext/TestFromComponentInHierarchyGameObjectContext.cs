using System;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings.FromComponentInHierarchyGameObjectContext
{
    public class TestFromComponentInHierarchyGameObjectContext : ZenjectIntegrationTestFixture
    {
        GameObject FooPrefab
        {
            get
            {
                return FixtureUtil.GetPrefab("TestFromComponentInHierarchyGameObjectContext/Foo");
            }
        }

        [SetUp]
        public void SetUp()
        {
            new GameObject().AddComponent<Gorp>();
            new GameObject().AddComponent<Gorp>();
        }

        [UnityTest]
        public IEnumerator TestCorrectHierarchy()
        {
            PreInstall();

            Container.Bind<Foo>().FromSubContainerResolve()
                .ByNewPrefab(FooPrefab).AsSingle().NonLazy();

            PostInstall();

            var foo = Container.Resolve<Foo>();

            Assert.IsNotNull(foo.Gorp);
            Assert.IsEqual(foo.gameObject.GetComponentsInChildren<Gorp>().Single(), foo.Gorp);
            yield break;
        }
    }
}
