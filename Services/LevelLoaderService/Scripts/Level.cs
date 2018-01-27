using System.Collections;
using System.Collections.Generic;
using Core.Audio;
using Core.Service;
using Core.UI;
using UniRx;
using UnityEngine;

namespace Core.LevelLoaderService
{
	public enum LevelState
	{
		Loaded,
		Started,
		InProgress,
		Completed
	}

	public abstract class Level : MonoBehaviour
	{
		public string LevelName { get { return name; } }

		public AudioPlayer backgroundMusic;

		protected ILevelLoaderService levelService;
		protected IAudioService audioService;
		protected IUIService uiService;

		protected LevelState levelState;
		public LevelState State { get { return levelState; } }

		protected virtual void Awake()
		{
			Debug.Log(("Level: " + LevelName + " loaded").Colored(Colors.lightblue));

			levelService = ServiceLocator.GetService<ILevelLoaderService>();
			audioService = ServiceLocator.GetService<IAudioService>();
			uiService = ServiceLocator.GetService<IUIService>();

			levelState = LevelState.Loaded;
		}

		protected virtual void Start()
		{
			Debug.Log(("Level: " + LevelName + " started").Colored(Colors.lightblue));

			levelState = LevelState.Started;

			// onLevelStarted.OnNext(this);
			levelState = LevelState.InProgress;

			if (audioService != null && backgroundMusic != null && backgroundMusic.Clip != null)
				audioService.PlayMusic(backgroundMusic);
		}

		public virtual void Unload()
		{
			if (audioService != null && backgroundMusic != null && backgroundMusic.Clip != null)
				audioService.StopClip(backgroundMusic);

			// onLevelUnloaded.OnNext(this);
		}

		protected virtual void OnDestroy()
		{
			if (audioService != null && backgroundMusic != null && backgroundMusic.Clip != null)
				audioService.StopClip(backgroundMusic);
		}
	}
}