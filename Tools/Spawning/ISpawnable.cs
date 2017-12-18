using System.Collections;
using System.Collections.Generic;
using Core.Signals;
using UnityEngine;

namespace Core.Spawning
{
	public interface ISpawnable
	{
		Signal<ISpawnable> Spawned { get; }
		Signal<ISpawnable> DeSpawned { get; }

		void Initialize();
		void DeInitialize();
	}
}
