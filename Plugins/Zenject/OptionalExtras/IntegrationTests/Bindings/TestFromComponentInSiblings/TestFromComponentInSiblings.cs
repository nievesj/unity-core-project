using System;
using System.Collections;
using UnityEngine.TestTools;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    public class TestFromComponentSibling : ZenjectIntegrationTestFixture
    {
        Foo _foo;
        Bar _bar;
        Qux _qux1;
        Qux _qux2;

        [SetUp]
        public void SetUp()
        {
            _foo = new GameObject().AddComponent<Foo>();

            _bar = _foo.gameObject.AddComponent<Bar>();
            _qux1 = _foo.gameObject.AddComponent<Qux>();
            _qux2 = _foo.gameObject.AddComponent<Qux>();
        }

        [UnityTest]
        public IEnumerator RunTest()
        {
            PreInstall();
            Container.Bind<Qux>().FromComponentSibling();
            Container.Bind<Bar>().FromComponentSibling();
            Container.Bind<IBar>().FromComponentSibling();

            PostInstall();

            Assert.IsEqual(_foo.Bar, _bar);
            Assert.IsEqual(_foo.IBar, _bar);
            Assert.IsEqual(_foo.Qux[0], _qux1);
            Assert.IsEqual(_foo.Qux[1], _qux2);

            // Should skip self
            Assert.IsEqual(_foo.Qux[0].OtherQux, _foo.Qux[1]);
            Assert.IsEqual(_foo.Qux[1].OtherQux, _foo.Qux[0]);
            yield break;
        }

        public class Qux : MonoBehaviour
        {
            [Inject]
            public Qux OtherQux;
        }

        public interface IBar
        {
        }

        public class Bar : MonoBehaviour, IBar
        {
        }

        public class Foo : MonoBehaviour
        {
            [Inject]
            public Bar Bar;

            [Inject]
            public IBar IBar;

            [Inject]
            public List<Qux> Qux;
        }
    }
}

