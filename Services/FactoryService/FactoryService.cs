using UnityEngine;
using Zenject;

namespace Core.Services.Factory
{
    public class FactoryService : Service
    {
#pragma warning disable 0414    // suppress value not used warning
        private FactoryServiceConfiguration _configuration;
#pragma warning restore 0414    // restore value not used warning

        private readonly DiContainer _diContainer;

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

        public Pooler<T> CreatePool<T>(Component prefab, int amount, Transform poolTransform = null) where T : Component
        {
            return new Pooler<T>(prefab, amount, _diContainer, poolTransform);
        }
    }
}