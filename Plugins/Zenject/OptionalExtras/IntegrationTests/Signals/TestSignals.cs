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
    public class TestSignals : ZenjectIntegrationTestFixture
    {
        void CommonInstall()
        {
            Container.BindInterfacesAndSelfTo<SignalManager>().AsSingle();
        }

        [UnityTest]
        public IEnumerator RunBasic()
        {
            PreInstall();
            CommonInstall();
            bool received = false;
            Container.DeclareSignal<SomethingHappenedSignal>();

            Container.BindSignal<SomethingHappenedSignal>().To(() => received = true);

            PostInstall();

            Assert.That(!received);
            Container.Resolve<SomethingHappenedSignal>().Fire();
            Assert.That(received);
            yield break;
        }

        [UnityTest]
        public IEnumerator RunTest()
        {
            PreInstall();
            CommonInstall();
            Container.Bind<Foo>().AsSingle();
            Container.Bind<Bar>().AsSingle();

            Container.DeclareSignal<SomethingHappenedSignal>();

            PostInstall();

            var foo = Container.Resolve<Foo>();
            var bar = Container.Resolve<Bar>();
            bar.Initialize();

            Assert.That(!bar.ReceivedSignal);
            foo.DoSomething();
            Assert.That(bar.ReceivedSignal);

            bar.Dispose();
            yield break;
        }

        [UnityTest]
        public IEnumerator RunTestSignalInterfaces()
        {
            PreInstall();
            CommonInstall();
            Container.DeclareSignal<SomethingHappenedSignal>();

            Container.DeclareSignal<AnotherSignal>();

            Container.Bind<IFooSignal>()
                .To(typeof(AnotherSignal), typeof(SomethingHappenedSignal)).FromResolve();

            PostInstall();

            var fooSignals = Container.ResolveAll<IFooSignal>();

            Assert.IsEqual(fooSignals.Count, 2);
            yield break;
        }

        public interface IFooSignal
        {
        }

        public class SomethingHappenedSignal : Signal<SomethingHappenedSignal>, IFooSignal
        {
        }

        public class AnotherSignal : Signal<AnotherSignal>, IFooSignal
        {
        }

        public class Foo
        {
            SomethingHappenedSignal _trigger;

            public Foo(SomethingHappenedSignal trigger)
            {
                _trigger = trigger;
            }

            public void DoSomething()
            {
                _trigger.Fire();
            }
        }

        public class Bar
        {
            SomethingHappenedSignal _signal;
            bool _receivedSignal;

            public Bar(SomethingHappenedSignal signal)
            {
                _signal = signal;
            }

            public bool ReceivedSignal
            {
                get
                {
                    return _receivedSignal;
                }
            }

            public void Initialize()
            {
                _signal += OnStarted;
            }

            public void Dispose()
            {
                _signal -= OnStarted;
            }

            void OnStarted()
            {
                _receivedSignal = true;
            }
        }
    }
}
