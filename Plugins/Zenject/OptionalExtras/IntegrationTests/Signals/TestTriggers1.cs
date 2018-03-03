using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ModestTree;
using UnityEngine.TestTools;
using Assert=ModestTree.Assert;
using Zenject;

namespace ZenjectSignalsAndSignals.Tests
{
    public class TestTriggers1 : ZenjectIntegrationTestFixture
    {
        void CommonInstall()
        {
            Container.BindInterfacesAndSelfTo<SignalManager>().AsSingle();
        }

        [UnityTest]
        public IEnumerator Test1()
        {
            PreInstall();
            CommonInstall();
            Container.DeclareSignal<FooSignal>();

            PostInstall();

            var signal = Container.Resolve<FooSignal>();

            bool received = false;
            signal += delegate { received = true; };

            // This is a compiler error
            //signal.Event();

            Assert.That(!received);
            signal.Fire();
            Assert.That(received);
            yield break;
        }

        public class FooSignal : Signal<FooSignal>
        {
        }
    }
}
