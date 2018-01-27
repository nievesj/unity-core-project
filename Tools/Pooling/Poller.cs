using System.Collections;
using System.Collections.Generic;
using MatchGame;
using UnityEngine;

namespace Core.Polling
{
	public class Poller<T> where T : Component
	{
		protected Stack<T> pool;
		protected GameObject prefab;

		protected Transform pollerTransform;
		public Transform PollerTransform { get { return pollerTransform; } }

		private int sizeLimit = 1;

		public Poller(GameObject prefab, int amount)
		{
			var go = new GameObject(Constants.PooledObject + prefab.name);
			pollerTransform = go.transform;
			this.prefab = prefab;
			sizeLimit = amount;

			CreatePool(amount);
		}

		public T Pop()
		{
			if (pool.Count == 0)
				return null;

			return Get();
		}

		public T PopResize()
		{
			sizeLimit++;
			if (pool.Count == 0)
				pool.Push(CreateObject(prefab));

			return Get();
		}

		private T Get()
		{
			T obj = pool.Pop();
			obj.gameObject.SetActive(true);

			return obj;
		}

		public void Push(T obj)
		{
			obj.gameObject.SetActive(false);

			if (!pool.Count.Equals(sizeLimit))
				pool.Push(obj);
			else
				GameObject.Destroy(obj.gameObject);
		}

		public void Destroy()
		{
			DestroyPool();

			if (pollerTransform)
				GameObject.Destroy(pollerTransform.gameObject);
		}

		protected void CreatePool(int amount)
		{
			if (pool != null)
				DestroyPool();

			pool = new Stack<T>();

			for (int i = 0; i <= amount - 1; i++)
				pool.Push(CreateObject(prefab));
		}

		protected T CreateObject(GameObject prefab)
		{
			GameObject go = GameObject.Instantiate(prefab, pollerTransform);
			go.SetActive(false);
			return go.GetComponent<T>()as T;
		}

		public void ResizePool(int val)
		{
			sizeLimit = val;
			if (pool.Count < val)
			{
				for (int i = pool.Count; i <= val - 1; i++)
					pool.Push(CreateObject(prefab));
			}
			else if (pool.Count > val)
			{
				for (;
					(pool.Count - 1)>= val;)
				{
					var o = pool.Pop();
					GameObject.Destroy(o.gameObject);
				}
			}
		}

		protected void DestroyPool()
		{
			foreach (var obj in pool)
			{
				if (obj)
					GameObject.Destroy(obj.gameObject);
			}
		}
	}
}