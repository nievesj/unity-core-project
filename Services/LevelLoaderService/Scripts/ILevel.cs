using System.Collections;
using System.Collections.Generic;
using Core.Service;
using Core.Signals;
using UnityEngine;

namespace Core.LevelLoaderService
{
	public interface ILevel
	{
		string LevelName { get; }
		LevelState State { get; }

		Signal<ILevel> OnLevelLoaded { get; }
		Signal<ILevel> OnLevelStarted { get; }
		Signal<ILevel> OnLevelCompleted { get; }
		Signal<ILevel> OnLevelUnloaded { get; }

		void Run();
		void CompleteLevel();
	}

	public enum LevelState
	{
		Loaded,
		Started,
		InProgress,
		Completed
	}
}