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

#if ZEN_SIGNALS_ADD_UNIRX
using UniRx;
#endif

namespace ZenjectSignalsAndSignals.Tests
{
    public class TestSignals2 : ZenjectIntegrationTestFixture
    {
        void CommonInstall()
        {
            Container.BindInterfacesAndSelfTo<SignalManager>().AsSingle();
        }

        [UnityTest]
        public IEnumerator TestToSingle1()
        {
            PreInstall();
            CommonInstall();
            Bar.TriggeredCount = 0;
            Bar.InstanceCount = 0;

            Container.Bind<Bar>().AsSingle();

            Container.DeclareSignal<DoSomethingSignal>();
            Container.BindSignal<DoSomethingSignal>()
                .To<Bar>(x => x.Execute).AsSingle();

            PostInstall();

            Container.Resolve<Bar>();
            var cmd = Container.Resolve<DoSomethingSignal>();

            Assert.IsEqual(Bar.InstanceCount, 1);
            Assert.IsEqual(Bar.TriggeredCount, 0);

            cmd.Fire();

            Assert.IsEqual(Bar.TriggeredCount, 1);
            Assert.IsEqual(Bar.TriggeredCount, 1);
            yield break;
        }

        [UnityTest]
        [ValidateOnly]
        public IEnumerator TestValidationFailure()
        {
            PreInstall();
            CommonInstall();
            Container.Bind<Qux>().AsSingle();
            Container.DeclareSignal<DoSomethingSignal>();
            Container.BindSignal<DoSomethingSignal>()
                .To<Bar>(x => x.Execute).FromResolve();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        [ValidateOnly]
        public IEnumerator TestValidationSuccess()
        {
            PreInstall();
            CommonInstall();
            Container.Bind<Qux>().AsSingle();
            Container.Bind<Bar>().AsSingle();
            Container.DeclareSignal<DoSomethingSignal>();
            Container.BindSignal<DoSomethingSignal>()
                .To<Bar>(x => x.Execute).FromResolve();

            PostInstall();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToCached1()
        {
            PreInstall();
            CommonInstall();
            Bar.TriggeredCount = 0;
            Bar.InstanceCount = 0;

            Container.Bind<Bar>().AsCached();

            Container.DeclareSignal<DoSomethingSignal>();
            Container.BindSignal<DoSomethingSignal>()
                .To<Bar>(x => x.Execute).AsCached();

            PostInstall();

            Container.Resolve<Bar>();
            var cmd = Container.Resolve<DoSomethingSignal>();

            Assert.IsEqual(Bar.InstanceCount, 1);
            Assert.IsEqual(Bar.TriggeredCount, 0);

            cmd.Fire();

            Assert.IsEqual(Bar.TriggeredCount, 1);
            Assert.IsEqual(Bar.InstanceCount, 2);

            cmd.Fire();
            Assert.IsEqual(Bar.InstanceCount, 2);
            Assert.IsEqual(Bar.TriggeredCount, 2);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestNoHandlerDefault()
        {
            PreInstall();
            CommonInstall();
            Container.DeclareSignal<DoSomethingSignal>();

            PostInstall();

            var cmd = Container.Resolve<DoSomethingSignal>();
            cmd.Fire();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestNoHandlerRequiredFailure()
        {
            PreInstall();
            CommonInstall();
            Container.DeclareSignal<DoSomethingSignal>().RequireHandler();

            PostInstall();

            var cmd = Container.Resolve<DoSomethingSignal>();

            Assert.Throws(() => cmd.Fire());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestNoHandlerRequiredSuccess()
        {
            PreInstall();
            CommonInstall();
            Container.DeclareSignal<DoSomethingSignal>().RequireHandler();
            Container.BindSignal<DoSomethingSignal>()
                .To<Bar>(x => x.Execute).AsCached();

            PostInstall();

            var cmd = Container.Resolve<DoSomethingSignal>();
            cmd.Fire();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestHandlerRequiredEventStyle()
        {
            PreInstall();
            CommonInstall();
            Container.DeclareSignal<DoSomethingSignal>().RequireHandler();

            PostInstall();

            var signal = Container.Resolve<DoSomethingSignal>();

            Assert.Throws(() => signal.Fire());

            bool received = false;

            Action receiveSignal = () => received = true;

            signal += receiveSignal;

            Assert.That(!received);
            signal.Fire();
            Assert.That(received);

            signal -= receiveSignal;

            Assert.Throws(() => signal.Fire());
            yield break;
        }

#if ZEN_SIGNALS_ADD_UNIRX
        [UnityTest]
        public IEnumerator TestHandlerRequiredUniRxStyle()
        {
            PreInstall();
            CommonInstall();
            Container.DeclareSignal<DoSomethingSignal>().RequireHandler();

            PostInstall();

            var signal = Container.Resolve<DoSomethingSignal>();

            Assert.Throws(() => signal.Fire());

            bool received = false;

            var subscription = signal.AsObservable.Subscribe((x) => received = true);

            Assert.That(!received);
            signal.Fire();
            Assert.That(received);

            subscription.Dispose();

            Assert.Throws(() => signal.Fire());
            yield break;
        }
#endif

        [UnityTest]
        public IEnumerator TestToMethod()
        {
            PreInstall();
            CommonInstall();
            bool wasCalled = false;

            Container.DeclareSignal<DoSomethingSignal>();
            Container.BindSignal<DoSomethingSignal>()
                .To(() => wasCalled = true);

            PostInstall();

            var cmd = Container.Resolve<DoSomethingSignal>();

            Assert.That(!wasCalled);
            cmd.Fire();
            Assert.That(wasCalled);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestMultipleHandlers()
        {
            PreInstall();
            CommonInstall();
            bool wasCalled1 = false;
            bool wasCalled2 = false;

            Container.DeclareSignal<DoSomethingSignal>();
            Container.BindSignal<DoSomethingSignal>()
                .To(() => wasCalled1 = true);
            Container.BindSignal<DoSomethingSignal>()
                .To(() => wasCalled2 = true);

            PostInstall();

            var cmd = Container.Resolve<DoSomethingSignal>();

            Assert.That(!wasCalled1);
            Assert.That(!wasCalled2);
            cmd.Fire();
            Assert.That(wasCalled1);
            Assert.That(wasCalled2);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewTransient()
        {
            PreInstall();
            CommonInstall();
            Bar.TriggeredCount = 0;
            Bar.InstanceCount = 0;

            Container.DeclareSignal<DoSomethingSignal>();
            Container.BindSignal<DoSomethingSignal>()
                .To<Bar>(x => x.Execute).AsTransient();

            PostInstall();

            TestBarHandlerTransient();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewCached()
        {
            PreInstall();
            CommonInstall();
            Bar.TriggeredCount = 0;
            Bar.InstanceCount = 0;

            Container.DeclareSignal<DoSomethingSignal>();
            Container.BindSignal<DoSomethingSignal>()
                .To<Bar>(x => x.Execute).AsCached();

            PostInstall();

            TestBarHandlerCached();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromNewSingle()
        {
            PreInstall();
            CommonInstall();
            Bar.TriggeredCount = 0;
            Bar.InstanceCount = 0;

            Container.DeclareSignal<DoSomethingSignal>();
            Container.BindSignal<DoSomethingSignal>()
                .To<Bar>(x => x.Execute).AsSingle();

            PostInstall();

            TestBarHandlerCached();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromMethod()
        {
            PreInstall();
            CommonInstall();
            Bar.TriggeredCount = 0;
            Bar.InstanceCount = 0;

            Container.DeclareSignal<DoSomethingSignal>();
            Container.BindSignal<DoSomethingSignal>()
                .To<Bar>(x => x.Execute).FromMethod(_ => new Bar());

            PostInstall();

            TestBarHandlerTransient();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromMethodMultiple()
        {
            PreInstall();
            CommonInstall();
            Bar.TriggeredCount = 0;
            Bar.InstanceCount = 0;

            Container.DeclareSignal<DoSomethingSignal>();
            Container.BindSignal<DoSomethingSignal>()
                .To<Bar>(x => x.Execute).FromMethodMultiple(_ => new[] { new Bar(), new Bar() });

            PostInstall();

            var cmd = Container.Resolve<DoSomethingSignal>();

            Assert.IsEqual(Bar.TriggeredCount, 0);
            Assert.IsEqual(Bar.InstanceCount, 0);

            cmd.Fire();

            Assert.IsEqual(Bar.TriggeredCount, 2);
            Assert.IsEqual(Bar.InstanceCount, 2);

            cmd.Fire();

            Assert.IsEqual(Bar.TriggeredCount, 4);
            Assert.IsEqual(Bar.InstanceCount, 4);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFromMethodMultipleEmpty()
        {
            PreInstall();
            CommonInstall();
            Bar.TriggeredCount = 0;
            Bar.InstanceCount = 0;

            Container.DeclareSignal<DoSomethingSignal>();
            Container.BindSignal<DoSomethingSignal>()
                .To<Bar>(x => x.Execute).FromMethodMultiple(_ => new Bar[] {});

            PostInstall();

            var signal = Container.Resolve<DoSomethingSignal>();

            Assert.Throws(() => signal.Fire());
            yield break;
        }

        void TestBarHandlerTransient()
        {
            var cmd = Container.Resolve<DoSomethingSignal>();

            Assert.IsEqual(Bar.TriggeredCount, 0);
            Assert.IsEqual(Bar.InstanceCount, 0);

            cmd.Fire();

            Assert.IsEqual(Bar.TriggeredCount, 1);
            Assert.IsEqual(Bar.InstanceCount, 1);

            cmd.Fire();

            Assert.IsEqual(Bar.InstanceCount, 2);
            Assert.IsEqual(Bar.TriggeredCount, 2);
        }

        void TestBarHandlerCached()
        {
            var cmd = Container.Resolve<DoSomethingSignal>();

            Assert.IsEqual(Bar.TriggeredCount, 0);
            Assert.IsEqual(Bar.InstanceCount, 0);

            cmd.Fire();

            Assert.IsEqual(Bar.TriggeredCount, 1);
            Assert.IsEqual(Bar.InstanceCount, 1);

            cmd.Fire();

            Assert.IsEqual(Bar.InstanceCount, 1);
            Assert.IsEqual(Bar.TriggeredCount, 2);
        }

        public class DoSomethingSignal : Signal<DoSomethingSignal>
        {
        }

        public class Qux
        {
            public Qux(Bar bar)
            {
            }
        }

        public class Bar
        {
            public static int InstanceCount = 0;

            public Bar()
            {
                InstanceCount++;
            }

            public static int TriggeredCount
            {
                get;
                set;
            }

            public void Execute()
            {
                TriggeredCount++;
            }
        }
    }
}
