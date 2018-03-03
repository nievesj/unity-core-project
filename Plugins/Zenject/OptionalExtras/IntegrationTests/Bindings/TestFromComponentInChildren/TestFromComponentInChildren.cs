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
    public class TestFromComponentInChildren : ZenjectIntegrationTestFixture
    {
        Root _root;
        Child _child1;
        Child _child2;
        Grandchild _grandchild;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("root").AddComponent<Root>();

            _child1 = new GameObject("child1").AddComponent<Child>();
            _child1.transform.SetParent(_root.transform);

            _child2 = new GameObject("child2").AddComponent<Child>();
            _child2.transform.SetParent(_root.transform);

            _grandchild = new GameObject("grandchild").AddComponent<Grandchild>();
            _grandchild.transform.SetParent(_child1.transform);
        }

        [UnityTest]
        public IEnumerator RunTest()
        {
            PreInstall();
            Container.Bind<Grandchild>().FromComponentInChildren();
            Container.Bind<Child>().FromComponentInChildren();

            PostInstall();

            Assert.IsEqual(_root.Grandchild, _grandchild);
            Assert.IsEqual(_root.Childs[0], _child1);
            Assert.IsEqual(_root.Childs[1], _child2);
            yield break;
        }

        public class Root : MonoBehaviour
        {
            [Inject]
            public Grandchild Grandchild;

            [Inject]
            public List<Child> Childs;
        }

        public class Child : MonoBehaviour
        {
        }

        public class Grandchild : MonoBehaviour
        {
        }
    }
}

