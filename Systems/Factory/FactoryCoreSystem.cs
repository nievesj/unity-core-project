using UnityEngine;
using Zenject;

namespace Core.Systems
{
    public class FactoryCoreSystem : CoreSystem
    {
        private DiContainer _diContainer;

        public void SetDiContainer(DiContainer context)
        {
            _diContainer = context;
        }
        
        public new T Instantiate<T>(T original) where T : Object
        {
            return _diContainer.InstantiatePrefabForComponent<T>(original);
        }
        
        public new T Instantiate<T>(T original, Transform trans) where T : Object
        {
            return _diContainer.InstantiatePrefabForComponent<T>(original, trans);
        }
        
        public new T Instantiate<T>(T original, Vector3 pos, Quaternion rotation, Transform trans = null) where T : Object
        {
            return _diContainer.InstantiatePrefabForComponent<T>(original, pos, rotation, trans);
        }
        
        public new GameObject Instantiate(Object original, Transform transform)
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