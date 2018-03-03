using System.Collections;
using System.Collections.Generic;
using Core.Polling;
using UnityEngine;
using Zenject;

namespace Core.Services.Factory
{
	public interface IFactoryService : IService { }

	public class FactoryService : IFactoryService
	{
		private FactoryServiceConfiguration _configuration;
		private readonly DiContainer _diContainer;

		public FactoryService(ServiceConfiguration config, DiContainer container)
		{
			_configuration = config as FactoryServiceConfiguration;
			_diContainer = container;
		}

		public T Instantiate<T>(T obj) where T : UnityEngine.Object
		{
			return _diContainer.InstantiatePrefabForComponent<T>(obj);
		}

		public T Instantiate<T>(T obj, Transform t) where T : UnityEngine.Object
		{
			return _diContainer.InstantiatePrefabForComponent<T>(obj, t);
		}

		public GameObject Instantiate(UnityEngine.Object obj, Transform t)
		{
			return _diContainer.InstantiatePrefab(obj, t);
		}

		public Pooler<T> CreatePool<T>(GameObject prefab, int amount) where T : UnityEngine.Component
		{
			return new Pooler<T>(prefab, amount, _diContainer);
		}
	}
}
