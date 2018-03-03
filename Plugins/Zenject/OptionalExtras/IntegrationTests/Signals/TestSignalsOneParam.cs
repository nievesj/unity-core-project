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
    public class TestSignalsOneParam : ZenjectIntegrationTestFixture
    {
        void CommonInstall()
        {
            Container.BindInterfacesAndSelfTo<SignalManager>().AsSingle();
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

            Assert.IsNull(bar.ReceivedValue);
            foo.DoSomething("asdf");
            Assert.IsEqual(bar.ReceivedValue, "asdf");

            bar.Dispose();
            yield break;
        }

        public class SomethingHappenedSignal : Signal<SomethingHappenedSignal, string>
        {
        }

        public class Foo
        {
            SomethingHappenedSignal _trigger;

            public Foo(SomethingHappenedSignal trigger)
            {
                _trigger = trigger;
            }

            public void DoSomething(string value)
            {
                _trigger.Fire(value);
            }
        }

        public class Bar
        {
            SomethingHappenedSignal _signal;
            string _receivedValue;

            public Bar(SomethingHappenedSignal signal)
            {
                _signal = signal;
            }

            public string ReceivedValue
            {
                get
                {
                    return _receivedValue;
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

            void OnStarted(string value)
            {
                _receivedValue = value;
            }
        }
    }
}
