using UnityEngine;
using Zenject;

namespace Core.Services.Factory
{
	public class FactoryService : Service
	{
		private FactoryServiceConfiguration _configuration;

		private DiContainer _diContainer;

		public FactoryService(ServiceConfiguration config, DiContainer context)
		{
			_configuration = config as FactoryServiceConfiguration;
			_diContainer = context;
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

		public Pooler<T> CreatePool<T>(GameObject prefab, int amount,  Transform poolTransform = null) where T : UnityEngine.Component
		{
			return new Pooler<T>(prefab, amount, _diContainer, poolTransform);
		}
	}
}
