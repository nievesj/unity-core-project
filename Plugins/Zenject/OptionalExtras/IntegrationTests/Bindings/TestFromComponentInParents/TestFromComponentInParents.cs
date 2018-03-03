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
    public class TestFromComponentInParents : ZenjectIntegrationTestFixture
    {
        Root _root;
        Child _child1;
        Child _child2;
        Child _child3;
        Grandchild _grandchild;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject().AddComponent<Root>();

            _child1 = new GameObject().AddComponent<Child>();
            _child1.transform.SetParent(_root.transform);

            _child2 = new GameObject().AddComponent<Child>();
            _child2.transform.SetParent(_child1.transform);
            _child3 = _child2.gameObject.AddComponent<Child>();

            _grandchild = new GameObject().AddComponent<Grandchild>();
            _grandchild.transform.SetParent(_child2.transform);
        }

        [UnityTest]
        public IEnumerator RunTest()
        {
            PreInstall();
            Container.Bind<Root>().FromComponentInParents();
            Container.Bind<Child>().FromComponentInParents();

            PostInstall();

            Assert.IsEqual(_grandchild.Childs[0], _child2);
            Assert.IsEqual(_grandchild.Childs[1], _child3);
            Assert.IsEqual(_grandchild.Childs[2], _child1);
            Assert.IsEqual(_grandchild.Root, _root);
            yield break;
        }

        public class Root : MonoBehaviour
        {
        }

        public class Child : MonoBehaviour
        {
        }

        public class Grandchild : MonoBehaviour
        {
            [Inject]
            public Root Root;

            [Inject]
            public List<Child> Childs;
        }
    }
}
