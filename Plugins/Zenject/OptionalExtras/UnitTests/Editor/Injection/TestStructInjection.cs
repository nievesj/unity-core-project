using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Injection
{
    [TestFixture]
    public class TestStructInjection : ZenjectUnitTestFixture
    {
        struct Test1
        {
        }

        class Test2
        {
            public Test2(Test1 t1)
            {
            }
        }

        [Test]
        public void TestCase1()
        {
            Container.Bind<Test1>().FromInstance(new Test1()).NonLazy();
            Container.Bind<Test2>().AsSingle().NonLazy();

            Container.ResolveDependencyRoots();

            var t2 = Container.Resolve<Test2>();

            Assert.That(t2 != null);
        }
    }
}

