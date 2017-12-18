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

		public Poller(GameObject prefab, int amount)
		{
			var go = new GameObject(Constants.PooledObject + prefab.name);
			pollerTransform = go.transform;
			this.prefab = prefab;

			CreatePool(amount);
		}

		public T Pop()
		{
			if (pool.Count == 0)
				pool.Push(CreateObject(prefab));

			T obj = pool.Pop();
			obj.gameObject.SetActive(true);

			return obj;
		}

		public void Push(T obj)
		{
			obj.gameObject.SetActive(false);
			pool.Push(obj);
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

			for (int i = 0; i <= amount; i++)
			{
				pool.Push(CreateObject(prefab));
			}
		}

		protected T CreateObject(GameObject prefab)
		{
			GameObject go = GameObject.Instantiate(prefab, pollerTransform);
			go.SetActive(false);
			return go.GetComponent<T>() as T;
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