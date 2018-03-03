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
    public class TestSignalsSubcontainers : ZenjectIntegrationTestFixture
    {
        void CommonInstall()
        {
            Container.BindInterfacesAndSelfTo<SignalManager>().AsSingle();
        }

        [UnityTest]
        public IEnumerator TestMissingDeclaration()
        {
            PreInstall();
            CommonInstall();
            Container.BindSignal<DoSomethingSignal>().To<Bar>(x => x.Execute).AsSingle();
            PostInstall();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestDeclarationBelowHandler()
        {
            PreInstall();
            CommonInstall();
            Bar.WasTriggered = false;

            Container.BindSignal<DoSomethingSignal>().To<Bar>(x => x.Execute).AsSingle();

            Container.BindInterfacesAndSelfTo<Foo>().FromSubContainerResolve().ByMethod(InstallFoo).AsSingle().NonLazy();

            PostInstall();

            Assert.Throws(() => Container.Resolve<DoSomethingSignal>());

            var foo = Container.Resolve<Foo>();

            Assert.That(!Bar.WasTriggered);
            foo.Trigger();
            Assert.That(Bar.WasTriggered);
            yield break;
        }

        static void InstallFoo(DiContainer container)
        {
            container.Bind<Foo>().AsSingle();
            container.DeclareSignal<DoSomethingSignal>();
        }

        [UnityTest]
        public IEnumerator TestDeclarationAboveHandler()
        {
            PreInstall();
            CommonInstall();
            Bar.WasTriggered = false;

            Container.DeclareSignal<DoSomethingSignal>();

            Container.BindInterfacesAndSelfTo<Foo>().FromSubContainerResolve().ByMethod(InstallFoo2).AsSingle().NonLazy();

            PostInstall();

            var cmd = Container.Resolve<DoSomethingSignal>();
            Container.Resolve<Foo>();

            Assert.That(!Bar.WasTriggered);
            cmd.Fire();
            Assert.That(Bar.WasTriggered);
            yield break;
        }

        static void InstallFoo2(DiContainer container)
        {
            container.Bind<Foo>().AsSingle();
            container.BindSignal<DoSomethingSignal>().To<Bar>(x => x.Execute).AsSingle();
        }

        public class Foo : Kernel
        {
            readonly DoSomethingSignal _command;

            public Foo(DoSomethingSignal command)
            {
                _command = command;
            }

            public void Trigger()
            {
                _command.Fire();
            }
        }

        public class DoSomethingSignal : Signal<DoSomethingSignal>
        {
        }

        public class Bar
        {
            public static bool WasTriggered
            {
                get;
                set;
            }

            public void Execute()
            {
                WasTriggered = true;
            }
        }
    }
}
