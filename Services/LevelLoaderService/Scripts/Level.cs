using System.Collections;
using System.Collections.Generic;
using Core.Audio;
using Core.Service;
using Core.Signals;
using Core.UI;
using UnityEngine;

namespace Core.LevelLoaderService
{
	public class Level : MonoBehaviour
	{
		public bool displayHUD;

		public string LevelName { get { return name; } }

		public AudioPlayer backgroundMusic;

		protected ILevelLoaderService levelService;
		protected IAudioService audioService;
		protected IUIService uiService;
		protected UIWindow hud;
		protected LevelState levelState;
		public LevelState State { get { return levelState; } }

		protected Signal<Level> onLevelLoaded = new Signal<Level>();
		public Signal<Level> OnLevelLoaded { get { return onLevelLoaded; } }

		protected Signal<Level> onLevelStarted = new Signal<Level>();
		public Signal<Level> OnLevelStarted { get { return onLevelStarted; } }

		protected Signal<Level> onLevelCompleted = new Signal<Level>();
		public Signal<Level> OnLevelCompleted { get { return onLevelCompleted; } }

		protected Signal<Level> onLevelUnloaded = new Signal<Level>();
		public Signal<Level> OnLevelUnloaded { get { return onLevelUnloaded; } }

		protected virtual void Awake()
		{
			Debug.Log(("Level: " + LevelName + " loaded").Colored(Colors.lightblue));

			levelService = Services.GetService<ILevelLoaderService>();
			audioService = Services.GetService<IAudioService>();
			uiService = Services.GetService<IUIService>();

			if (uiService != null)
			{
				uiService.OnWindowOpened.Add(OnWindowOpened);
				uiService.OnWindowClosed.Add(OnWindowClosed);
			}

			levelState = LevelState.Loaded;
			LevelLoaded();
		}

		protected virtual void Start()
		{
			Debug.Log(("Level: " + LevelName + " started").Colored(Colors.lightblue));

			levelState = LevelState.Started;

			onLevelStarted.Dispatch(this);
			levelState = LevelState.InProgress;

			if (audioService != null && backgroundMusic != null && backgroundMusic.Clip != null)
				audioService.PlayMusic(backgroundMusic);

			if (uiService != null && displayHUD)
				uiService.OpenHUD();
		}

		protected virtual void LevelLoaded()
		{
			onLevelLoaded.Dispatch(this);
		}

		/// <summary>
		/// Completing the level will unload it, and completely remove it from memory, and nextLevel will start loading
		/// </summary>
		public virtual void CompleteLevel()
		{
			Debug.Log(("Level: " + LevelName + " completed").Colored(Colors.lightblue));

			levelState = LevelState.Completed;
			onLevelCompleted.Dispatch(this);
		}

		public virtual void Unload()
		{
			if (audioService != null && backgroundMusic != null && backgroundMusic.Clip != null)
				audioService.StopClip(backgroundMusic);

			onLevelUnloaded.Dispatch(this);
		}

		protected virtual void OnWindowOpened(UIWindow window)
		{
			if (window is UIHud)
				hud = window;
		}

		protected virtual void OnWindowClosed(UIWindow window)
		{
			if (window is UIHud)
			{
				uiService.OnWindowOpened.Remove(OnWindowOpened);
				uiService.OnWindowClosed.Remove(OnWindowClosed);
			}
		}

		protected virtual void OnDestroy()
		{
			if (hud)
				hud.Close();

			if (audioService != null && backgroundMusic != null && backgroundMusic.Clip != null)
				audioService.StopClip(backgroundMusic);
		}
	}
}