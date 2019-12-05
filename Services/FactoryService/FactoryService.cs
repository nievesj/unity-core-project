using Core.Factory;
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

        public T Instantiate<T>(T original) where T : Object
        {
            return _diContainer.InstantiatePrefabForComponent<T>(original);
        }

        public T Instantiate<T>(T original, Transform transform) where T : Object
        {
            return _diContainer.InstantiatePrefabForComponent<T>(original, transform);
        }

        public GameObject Instantiate(Object original, Transform transform)
        {
            return _diContainer.InstantiatePrefab(original, transform);
        }

        public ComponentPool<T> CreateComponentPool<T>(Component prefab, int amount, Transform poolTransform = null) where T : Component
        {
            return new ComponentPool<T>(prefab, amount, _diContainer, poolTransform);
        }

        public PooledCoreBehaviourPool<T> CreatePooledCoreBehaviourPool<T>(Component prefab, int amount, Transform poolTransform = null) where T : Component, IInitializablePoolElement
        {
            return new PooledCoreBehaviourPool<T>(prefab, amount, _diContainer, poolTransform);
        }
    }
}