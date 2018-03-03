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
    public class TestFromSiblingComponent : ZenjectIntegrationTestFixture
    {
        [UnityTest]
        public IEnumerator TestBasic()
        {
            PreInstall();
            Container.Bind<Bar>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.Bind<Foo>().FromNewComponentSibling();

            PostInstall();

            Assert.IsEqual(Container.Resolve<Bar>().gameObject.GetComponents<Foo>().Length, 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInvalidUse()
        {
            PreInstall();
            Container.Bind<Qux>().AsSingle().NonLazy();
            Container.Bind<Foo>().FromNewComponentSibling();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestBasic2()
        {
            PreInstall();
            var gameObject = Container.CreateEmptyGameObject("Test");

            Container.Bind<Gorp>().FromNewComponentOn(gameObject).AsSingle().NonLazy();
            Container.Bind<Bar>().FromNewComponentOn(gameObject).AsSingle().NonLazy();

            Container.Bind<Foo>().FromNewComponentSibling();

            PostInstall();

            var bar = Container.Resolve<Bar>();
            var gorp = Container.Resolve<Gorp>();

            Assert.IsEqual(bar.gameObject.GetComponents<Foo>().Length, 1);
            Assert.IsEqual(bar.Foo, gorp.Foo);
            yield break;
        }

        public class Qux
        {
            public Qux(Foo foo)
            {
            }
        }

        public class Foo : MonoBehaviour
        {
        }

        public class Bar : MonoBehaviour
        {
            [Inject]
            public Foo Foo;
        }

        public class Gorp : MonoBehaviour
        {
            [Inject]
            public Foo Foo;
        }
    }
}
