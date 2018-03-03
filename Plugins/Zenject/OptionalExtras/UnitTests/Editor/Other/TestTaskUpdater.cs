using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using Zenject;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;
using ModestTree.Util;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestTaskUpdater
    {
        DiContainer _container;

        [SetUp]
        public void Setup()
        {
            _container = new DiContainer();

            _container.Bind<TaskUpdater<ITickable>>().FromInstance(new TickablesTaskUpdater());
        }

        public void BindTickable<TTickable>(int priority) where TTickable : ITickable
        {
            _container.Bind<ITickable>().To<TTickable>().AsSingle();
            _container.Bind<ModestTree.Util.ValuePair<Type, int>>().FromInstance(ModestTree.Util.ValuePair.New(typeof(TTickable), priority));
        }

        [Test]
        public void TestTickablesAreOptional()
        {
            Assert.IsNotNull(_container.Resolve<TaskUpdater<ITickable>>());
        }

        [Test]
        // Test that tickables get called in the correct order
        public void TestOrder()
        {
            _container.Bind<Tickable1>().AsSingle();
            _container.Bind<Tickable2>().AsSingle();
            _container.Bind<Tickable3>().AsSingle();

            BindTickable<Tickable3>(2);
            BindTickable<Tickable1>(0);
            BindTickable<Tickable2>(1);

            var taskUpdater = _container.Resolve<TaskUpdater<ITickable>>();

            var tick1 = _container.Resolve<Tickable1>();
            var tick2 = _container.Resolve<Tickable2>();
            var tick3 = _container.Resolve<Tickable3>();

            int tickCount = 0;

            tick1.TickCalled += delegate
            {
                Assert.IsEqual(tickCount, 0);
                tickCount++;
            };

            tick2.TickCalled += delegate
            {
                Assert.IsEqual(tickCount, 1);
                tickCount++;
            };

            tick3.TickCalled += delegate
            {
                Assert.IsEqual(tickCount, 2);
                tickCount++;
            };

            taskUpdater.UpdateAll();
        }

        class Tickable1 : ITickable
        {
            public event Action TickCalled = delegate {};

            public void Tick()
            {
                TickCalled();
            }
        }

        class Tickable2 : ITickable
        {
            public event Action TickCalled = delegate {};

            public void Tick()
            {
                TickCalled();
            }
        }

        class Tickable3 : ITickable
        {
            public event Action TickCalled = delegate {};

            public void Tick()
            {
                TickCalled();
            }
        }
    }
}
