using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using ModestTree;
using UnityEngine.TestTools;
using Assert=ModestTree.Assert;
using Zenject.Tests.Installers.ScriptableObjectInstallers;

namespace Zenject.Tests.Installers
{
    public class TestScriptableObjectInstallers : ZenjectIntegrationTestFixture
    {
        [UnityTest]
        public IEnumerator TestBadResourcePath()
        {
            PreInstall();
            Assert.Throws(() => FooInstaller.InstallFromResource("TestScriptableObjectInstallers/SDFSDFSDF", Container));
            PostInstall();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestZeroArgs()
        {
            PreInstall();
            FooInstaller.InstallFromResource("TestScriptableObjectInstallers/FooInstaller", Container);

            PostInstall();

            FixtureUtil.AssertResolveCount<Foo>(Container, 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestOneArg()
        {
            PreInstall();
            BarInstaller.InstallFromResource("TestScriptableObjectInstallers/BarInstaller", Container, "blurg");

            PostInstall();

            Assert.IsEqual(Container.Resolve<string>(), "blurg");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestThreeArgs()
        {
            PreInstall();
            QuxInstaller.InstallFromResource("TestScriptableObjectInstallers/QuxInstaller", Container, "blurg", 2.0f, 1);

            PostInstall();

            Assert.IsEqual(Container.Resolve<string>(), "blurg");
            yield break;
        }
    }
}
