using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using ModestTree;
using UnityEngine.TestTools;
using Assert=ModestTree.Assert;
using Zenject.Tests.Bindings.DiContainerMethods;

namespace Zenject.Tests
{
    public class TestIntegrationTest : ZenjectIntegrationTestFixture
    {
        public class Foo : IInitializable, IDisposable
        {
            public static bool WasDisposed
            {
                get; set;
            }

            public static bool WasInitialized
            {
                get; set;
            }

            public void Initialize()
            {
                WasInitialized = true;
            }

            public void Dispose()
            {
                WasDisposed = true;
            }
        }

        [UnityTest]
        public IEnumerator TestRun()
        {
            PreInstall();

            Foo.WasDisposed = false;
            Foo.WasInitialized = false;

            Container.BindInterfacesTo<Foo>().AsSingle();

            Assert.That(!Foo.WasInitialized);

            PostInstall();

            Assert.That(Foo.WasInitialized);

            DestroyAll();

            Assert.That(Foo.WasDisposed);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSkipInstall()
        {
            SkipInstall();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestProjectContextDestroyed()
        {
            Assert.That(!ProjectContext.HasInstance);
            SkipInstall();
            Assert.That(ProjectContext.HasInstance);
            DestroyAll();
            Assert.That(!ProjectContext.HasInstance);
            yield break;
        }
    }
}
