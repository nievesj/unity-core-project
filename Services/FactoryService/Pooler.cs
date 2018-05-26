using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core.Services.Factory
{
    public class Pooler<T> where T : Component
    {
        private Stack<T> _pool;

        private int _activeElements = 0;

        private GameObject _prefab;

        private DiContainer _diContainer;

        private Transform _poolerTransform;

        public Transform PoolerTransform => _poolerTransform;

        private int _sizeLimit = 1;

        public int SizeLimit => _sizeLimit;

        public int ActiveElements => _activeElements;

	    /// <summary>
	    ///     Initialize pooler
	    /// </summary>
	    /// <param name="pre">    Gameobject to be pooled </param>
	    /// <param name="amount"> Pool size </param>
	    public Pooler(GameObject pre, int amount, DiContainer container, Transform poolTransform = null)
        {
            if (poolTransform)
                _poolerTransform = poolTransform;
            else
            {
                var go = new GameObject(Constants.PooledObject + pre.name);
                _poolerTransform = go.transform;
            }
            
            _prefab = pre;
            _sizeLimit = amount;
            _diContainer = container;

            CreatePool(amount);
        }

	    /// <summary>
	    ///     Get element from the _pool
	    /// </summary>
	    /// <returns></returns>
	    public T Pop()
        {
            if (_pool.Count == 0)
                return null;

            return Get();
        }

	    /// <summary>
	    ///     Get element from the _pool, if there are no more elements allocated, create a new one
	    /// </summary>
	    /// <returns></returns>
	    public T PopResize()
        {
            _sizeLimit++;
            if (_pool.Count == 0)
                _pool.Push(CreateObject(_prefab));

            return Get();
        }

	    /// <summary>
	    ///     Resize _pool.This changes the size of the _pool, however, if there are elements alive
	    ///     that have been pooled, they will stay alive until they are pushed back into the _pool at
	    ///     which moment they will be destroyed if they dont fit in the ne wpool size
	    /// </summary>
	    /// <param name="val"> New _pool size </param>
	    public void ResizePool(int val)
        {
            _sizeLimit = val;
            var totalElems = _pool.Count + _activeElements;
            if (totalElems < val)
                for (var i = totalElems; i <= val - 1; i++)
                    _pool.Push(CreateObject(_prefab));
            else if (totalElems > val)
                for (var i = 0; i <= totalElems - val - 1; i++)
                    if (_pool.Count > 0)
                    {
                        var o = _pool.Pop();
                        GameObject.Destroy(o.gameObject);
                    }
        }

	    /// <summary>
	    ///     Return element to the _pool
	    /// </summary>
	    /// <param name="obj"></param>
	    public void Push(T obj)
        {
            obj.gameObject.SetActive(false);
            if (_pool.Count + _activeElements <= _sizeLimit)
                _pool.Push(obj);
            else
                GameObject.Destroy(obj.gameObject);

            _activeElements--;
        }

	    /// <summary>
	    ///     Destroy _pool
	    /// </summary>
	    public void Destroy()
        {
            DestroyPool();

            if (_poolerTransform)
                GameObject.Destroy(_poolerTransform.gameObject);
        }

        private T Get()
        {
            var obj = _pool.Pop();
            obj.gameObject.SetActive(true);
            _activeElements++;
            ;

            return obj;
        }

        private void CreatePool(int amount)
        {
            if (_pool != null)
                DestroyPool();

            _pool = new Stack<T>();
            _activeElements = 0;

            for (var i = 0; i <= amount - 1; i++)
                _pool.Push(CreateObject(_prefab));
        }

        private T CreateObject(GameObject prefab)
        {
            var go = _diContainer.InstantiatePrefab(prefab, _poolerTransform);
            go.SetActive(false);
            return go.GetComponent<T>() as T;
        }

        private void DestroyPool()
        {
            foreach (var obj in _pool)
                if (obj)
                    GameObject.Destroy(obj.gameObject);
        }
    }
}