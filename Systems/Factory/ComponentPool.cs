using System.Collections.Generic;
using Core.Common.Extensions.UnityComponent;
using UnityEngine;
using Zenject;

namespace Core.Systems
{
    public interface IInitializablePoolElement
    {
        void PoolElementWakeUp();
        void PoolElementSleep();
    }

    public class PoolContainer<T> where T : Component
    {
        protected Dictionary<string, ComponentPool<T>> _poolDictionary;

        public PoolContainer()
        {
            _poolDictionary = new Dictionary<string, ComponentPool<T>>();
        }

        public virtual void Add(string name, ComponentPool<T> pool)
        {
            if (!_poolDictionary.ContainsKey(name))
                _poolDictionary.Add(name, pool);
        }

        public virtual void Remove(string name)
        {
            if (_poolDictionary.ContainsKey(name))
                _poolDictionary.Remove(name);
        }

        public virtual ComponentPool<T> Get(string name)
        {
            if (_poolDictionary.ContainsKey(name))
                return _poolDictionary[name];

            return null;
        }

        public virtual TC Pop<TC>(string name) where TC : Component
        {
            if (_poolDictionary.ContainsKey(name))
            {
                var pop = _poolDictionary[name].Pop();
                if (pop)
                    return pop.GetComponent<TC>();
            }

            return null;
        }

        public virtual void Push(string name, T component)
        {
            if (_poolDictionary.ContainsKey(name))
                _poolDictionary[name].Push(component);
        }
    }

    public class PooledCoreBehaviourPool<T> : ComponentPool<T> where T : Component, IInitializablePoolElement
    {
        public PooledCoreBehaviourPool(Component prefab, int amount, DiContainer container, Transform poolTransform = null) : base(prefab, amount, container, poolTransform) { }

        public override void Push(T obj)
        {
            obj.gameObject.SetActive(false);
            if (_pool.Count + ActiveElements <= SizeLimit)
            {
                obj.PoolElementSleep();
                _pool.Push(obj);
            }
            else
                Object.Destroy(obj.gameObject);

            ActiveElements--;
        }

        protected override T Get()
        {
            var obj = _pool.Pop();
            obj.gameObject.SetActive(true);
            ActiveElements++;

            obj.PoolElementWakeUp();

            return obj;
        }
    }

    public class ComponentPool<T> where T : Component
    {
        public Transform PoolerTransform { get; }

        public int SizeLimit { get; protected set; }

        public int ActiveElements { get; protected set; } = 0;
        public IEnumerable<T> PoolElements => _pool;
        public bool HasElements => _pool.Count > 0;

        protected Stack<T> _pool;
        protected readonly Component _prefab;
        protected readonly DiContainer _diContainer;

        /// <summary>
        /// Initialize pool
        /// </summary>
        /// <param name="prefab"> Gameobject to be pooled </param>
        /// <param name="amount"> Pool size </param>
        /// <param name="container"></param>
        /// <param name="poolTransform"></param>
        public ComponentPool(Component prefab, int amount, DiContainer container, Transform poolTransform = null)
        {
            if (poolTransform)
                PoolerTransform = poolTransform;
            else
            {
                var go = new GameObject(Constants.PooledObject + prefab.name);
                PoolerTransform = go.transform;
            }

            _prefab = prefab;
            SizeLimit = amount;
            _diContainer = container;

            CreatePool(amount);
        }

        /// <summary>
        /// Get element from the _pool
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            if (_pool.Count == 0)
                return null;

            return Get();
        }

        /// <summary>
        /// Get element from the _pool, if there are no more elements allocated, create a new one
        /// </summary>
        /// <returns></returns>
        public T PopResize()
        {
            SizeLimit++;
            if (_pool.Count == 0)
                _pool.Push(CreateObject(_prefab));

            return Get();
        }

        /// <summary>
        /// Resize _pool.This changes the size of the _pool, however, if there are elements alive
        /// that have been pooled, they will stay alive until they are pushed back into the _pool at
        /// which moment they will be destroyed if they dont fit in the new pool size
        /// </summary>
        /// <param name="val"> New _pool size </param>
        public void ResizePool(int val)
        {
            SizeLimit = val;
            var totalElems = _pool.Count + ActiveElements;
            if (totalElems < val)
                for (var i = totalElems; i <= val - 1; i++)
                    _pool.Push(CreateObject(_prefab));
            else if (totalElems > val)
                for (var i = 0; i <= totalElems - val - 1; i++)
                    if (_pool.Count > 0)
                    {
                        var o = _pool.Pop();
                        Object.Destroy(o.gameObject);
                    }
        }

        /// <summary>
        /// Return element to the _pool
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Push(T obj)
        {
            obj.gameObject.SetActive(false);
            if (_pool.Count + ActiveElements <= SizeLimit)
                _pool.Push(obj);
            else
                Object.Destroy(obj.gameObject);

            ActiveElements--;
        }

        /// <summary>
        /// Destroy _pool
        /// </summary>
        public void Destroy()
        {
            DestroyPool();

            if (PoolerTransform)
                Object.Destroy(PoolerTransform.gameObject);
        }

        protected virtual T Get()
        {
            var obj = _pool.Pop();
            obj.gameObject.SetActive(true);
            ActiveElements++;

            return obj;
        }

        protected void CreatePool(int amount)
        {
            if (_pool != null)
                DestroyPool();

            _pool = new Stack<T>();
            ActiveElements = 0;

            for (var i = 0; i <= amount - 1; i++)
                _pool.Push(CreateObject(_prefab));
        }

        protected T CreateObject(Object prefab)
        {
            var go = _diContainer.InstantiatePrefab(prefab, PoolerTransform);
            go.SetActive(false);
            return go.GetComponent<T>() as T;
        }

        protected void DestroyPool()
        {
            foreach (var obj in _pool)
            {
                if (obj) obj.gameObject.Destroy();
            }
        }
    }
}