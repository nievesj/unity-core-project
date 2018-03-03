using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Injection
{
    [TestFixture]
    public class TestListInjection : ZenjectUnitTestFixture
    {
        class Test1
        {
            public Test1(List<int> values)
            {
            }
        }

        class Test3
        {
            [Inject]
            public List<int> values = null;
        }

        [Test]
        public void TestCase1()
        {
            Container.Bind<Test1>().AsSingle();

            Container.ResolveAll<Test1>();
        }

        [Test]
        public void TestCase3()
        {
            Container.Bind<Test3>().AsSingle();

            Container.ResolveAll<Test3>();
        }
    }
}



