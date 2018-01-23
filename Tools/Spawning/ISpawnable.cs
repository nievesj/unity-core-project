using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Spawning
{
	public interface ISpawnable
	{
		IObservable<ISpawnable> Spawned { get; }
		IObservable<ISpawnable> DeSpawned { get; }

		void Initialize();
		void DeInitialize();
	}
}