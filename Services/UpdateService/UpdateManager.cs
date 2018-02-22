using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.UpdateManager
{
	public class UpdateManager : MonoBehaviour
	{
		private BehaviourDelegateType[] update;
		private BehaviourDelegateType[] fixedUpdate;
		private BehaviourDelegateType[] lateUpdate;

		private int updateCount;
		private int fixedUpdateCount;
		private int lateUpdateCount;

		private void Awake()
		{
			update = new BehaviourDelegateType[0];
			fixedUpdate = new BehaviourDelegateType[0];
			lateUpdate = new BehaviourDelegateType[0];
		}

		private void Update()
		{
			if (updateCount == 0)
				return;

			for (int i = 0; i < updateCount; i++)
			{
				if (!update[i].behaviour)
					continue;

				update[i].method();
			}
		}

		private void FixedUpdate()
		{
			if (fixedUpdateCount == 0)
				return;

			for (int i = 0; i < fixedUpdateCount; i++)
			{
				if (!fixedUpdate[i].behaviour)
					continue;

				fixedUpdate[i].method();
			}
		}

		private void LateUpdate()
		{
			if (lateUpdateCount == 0)
				return;

			for (int i = 0; i < lateUpdateCount; i++)
			{
				if (!lateUpdate[i].behaviour)
					continue;

				lateUpdate[i].method();
			}
		}

		public void Attach(BehaviourDelegateType behaviourDelegateType)
		{
			if (!Contains(GetArray(behaviourDelegateType.type), behaviourDelegateType))
			{
				switch (behaviourDelegateType.type)
				{
					case UpdateType.Update:
						update = Add(update, behaviourDelegateType);
						updateCount++;
						break;
					case UpdateType.FidexUpdate:
						fixedUpdate = Add(fixedUpdate, behaviourDelegateType);
						fixedUpdateCount++;
						break;
					case UpdateType.LateUpdate:
						lateUpdate = Add(lateUpdate, behaviourDelegateType);
						lateUpdateCount++;
						break;
				}
			}
		}

		public void Detach(BehaviourDelegateType behaviourDelegateType)
		{
			if (Contains(GetArray(behaviourDelegateType.type), behaviourDelegateType))
			{
				switch (behaviourDelegateType.type)
				{
					case UpdateType.Update:
						update = Remove(update, behaviourDelegateType);
						updateCount--;
						break;
					case UpdateType.FidexUpdate:
						fixedUpdate = Remove(update, behaviourDelegateType);
						fixedUpdateCount--;
						break;
					case UpdateType.LateUpdate:
						lateUpdate = Remove(update, behaviourDelegateType);
						lateUpdateCount--;
						break;
				}
			}
		}

		private bool Contains(BehaviourDelegateType[] array, BehaviourDelegateType behaviour)
		{
			int length = array.Length;

			for (int i = 0; i < length; i++)
			{
				if (behaviour == array[i])
					return true;
			}

			return false;
		}

		private BehaviourDelegateType[] GetArray(UpdateType type)
		{
			switch (type)
			{
				case UpdateType.Update:
					return update;
				case UpdateType.FidexUpdate:
					return fixedUpdate;
				case UpdateType.LateUpdate:
					return lateUpdate;
			}

			return null;
		}

		private bool Contains(CoreBehaviour[] array, CoreBehaviour behaviour)
		{
			int length = array.Length;

			for (int i = 0; i < length; i++)
			{
				if (behaviour == array[i])
					return true;
			}

			return false;
		}

		private BehaviourDelegateType[] Add(BehaviourDelegateType[] array, BehaviourDelegateType behaviour)
		{
			int length = array.Length;
			BehaviourDelegateType[] extendedArray = new BehaviourDelegateType[length + 1];

			for (int i = 0; i < length; i++)
				extendedArray[i] = array[i];

			extendedArray[extendedArray.Length - 1] = behaviour;
			return extendedArray;
		}

		private BehaviourDelegateType[] Remove(BehaviourDelegateType[] array, BehaviourDelegateType behaviour)
		{
			int length = array.Length;
			BehaviourDelegateType[] retractedArray = new BehaviourDelegateType[length - 1];

			for (int i = 0; i < length; i++)
			{
				if (array[i] == behaviour)
					continue;

				retractedArray[i] = array[i];
			}

			return retractedArray;
		}
	}
}