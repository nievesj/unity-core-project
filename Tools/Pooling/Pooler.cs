using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Polling
{
	public class Pooler<T> where T : Component
	{
		protected Stack<T> pool;
		protected int activeElements = 0;

		protected GameObject prefab;

		protected Transform poolerTransform;
		public Transform PoolerTransform { get { return poolerTransform; } }

		private int sizeLimit = 1;

		/// <summary>
		/// Initialize pooler
		/// </summary>
		/// <param name="pre">Gameobject to be pooled</param>
		/// <param name="amount">Pool size</param>
		public Pooler(GameObject pre, int amount)
		{
			var go = new GameObject(Constants.PooledObject + pre.name);
			poolerTransform = go.transform;
			prefab = pre;
			sizeLimit = amount;

			CreatePool(amount);
		}

		/// <summary>
		/// Get element from the pool
		/// </summary>
		/// <returns></returns>
		public T Pop()
		{
			if (pool.Count == 0)
				return null;

			return Get();
		}

		/// <summary>
		/// Get element from the pool, if there are no more elements allocated, create a new one
		/// </summary>
		/// <returns></returns>
		public T PopResize()
		{
			sizeLimit++;
			if (pool.Count == 0)
				pool.Push(CreateObject(prefab));

			return Get();
		}

		/// <summary>
		/// Resize pool.This changes the size of the pool, however, if there are elements alive that
		/// have been pooled, they will stay alive until they are pushed back into the pool
		/// at which moment they will be destroyed if they dont fit in the ne wpool size
		/// </summary>
		/// <param name="val">New pool size</param>
		public void ResizePool(int val)
		{
			//TODO this logic is bad
			sizeLimit = val;
			var totalElems = pool.Count + activeElements;

			Debug.Log(totalElems + " " + val);
			Debug.Log(totalElems - val);
			if (totalElems < val)
			{
				for (int i = totalElems; i <= val - 1; i++)
					pool.Push(CreateObject(prefab));
			}
			else if (totalElems > val)
			{
				for (int i = 0; i <= (totalElems - val - 1); i++)
				{
					if (pool.Count > 0)
					{
						var o = pool.Pop();
						GameObject.Destroy(o.gameObject);
					}
				}
			}
		}

		/// <summary>
		/// Return element to the pool
		/// </summary>
		/// <param name="obj"></param>
		public void Push(T obj)
		{
			obj.gameObject.SetActive(false);
			if ((pool.Count + activeElements)<= sizeLimit)
				pool.Push(obj);
			else
				GameObject.Destroy(obj.gameObject);

			activeElements--;
		}

		/// <summary>
		/// Destroy pool
		/// </summary>
		public void Destroy()
		{
			DestroyPool();

			if (poolerTransform)
				GameObject.Destroy(poolerTransform.gameObject);
		}

		private T Get()
		{
			T obj = pool.Pop();
			obj.gameObject.SetActive(true);
			activeElements++;;

			return obj;
		}

		private void CreatePool(int amount)
		{
			if (pool != null)
				DestroyPool();

			pool = new Stack<T>();
			activeElements = 0;

			for (int i = 0; i <= amount - 1; i++)
				pool.Push(CreateObject(prefab));
		}

		private T CreateObject(GameObject prefab)
		{
			GameObject go = GameObject.Instantiate(prefab, poolerTransform);
			go.SetActive(false);
			return go.GetComponent<T>()as T;
		}

		private void DestroyPool()
		{
			foreach (var obj in pool)
			{
				if (obj)
					GameObject.Destroy(obj.gameObject);
			}
		}
	}
}