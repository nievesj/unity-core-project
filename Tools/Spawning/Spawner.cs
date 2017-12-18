using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Spawning
{
	public class Spawner : MonoBehaviour
	{
		[SerializeField]
		protected GameObject prefab;
		[SerializeField]
		protected bool flipX, flipY, flipZ;

		protected ISpawnable spawnedObject;


		public ISpawnable Spawn()
		{
			GameObject go = Instantiate(prefab, transform.position, Quaternion.identity);

			go.transform.SetParent(transform);

			spawnedObject = go.GetComponent<ISpawnable>();
			return spawnedObject;
		}

		protected Vector3 Flip(Transform trans)
		{
			Vector3 ret = trans.localScale;
			if (flipX)
				ret = new Vector3(-trans.localScale.x, trans.localScale.y, trans.localScale.z);

			if (flipY)
				ret = new Vector3(trans.localScale.x, -trans.localScale.y, trans.localScale.z);

			if (flipZ)
				ret = new Vector3(trans.localScale.x, trans.localScale.y, -trans.localScale.z);

			return ret;
		}

		public virtual void DeSpawn()
		{

		}
	}
}
