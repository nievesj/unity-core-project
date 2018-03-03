using System;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    public class TestFromComponentInHierarchy : ZenjectIntegrationTestFixture
    {
        Foo _foo;
        Bar _bar;

        [SetUp]
        public void SetUp()
        {
            var root = new GameObject();

            var child1 = new GameObject();
            child1.transform.SetParent(root.transform);

            var child2 = new GameObject();
            child2.transform.SetParent(root.transform);

            _foo = child2.AddComponent<Foo>();
            _bar = child2.AddComponent<Bar>();
        }

        [UnityTest]
        public IEnumerator RunTest()
        {
            PreInstall();
            Container.Bind<Foo>().FromComponentInHierarchy();

            PostInstall();

            Assert.IsEqual(_bar.Foo, _foo);
            yield break;
        }

        public class Foo : MonoBehaviour
        {
        }

        public class Bar : MonoBehaviour
        {
            [Inject]
            public Foo Foo;
        }
    }
}

