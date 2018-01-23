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

	public class Level : MonoBehaviour
	{
		public bool displayHUD;

		public string LevelName { get { return name; } }

		public AudioPlayer backgroundMusic;

		protected ILevelLoaderService levelService;
		protected IAudioService audioService;
		protected IUIService uiService;
		protected UIWindow hud;
		protected CompositeDisposable disposables = new CompositeDisposable();

		protected LevelState levelState;
		public LevelState State { get { return levelState; } }

		protected Subject<Level> onLevelLoaded = new Subject<Level>();
		public IObservable<Level> OnLevelLoaded { get { return onLevelLoaded; } }

		protected Subject<Level> onLevelStarted = new Subject<Level>();
		public IObservable<Level> OnLevelStarted { get { return onLevelStarted; } }

		protected Subject<Level> onLevelCompleted = new Subject<Level>();
		public IObservable<Level> OnLevelCompleted { get { return onLevelCompleted; } }

		protected Subject<Level> onLevelUnloaded = new Subject<Level>();
		public IObservable<Level> OnLevelUnloaded { get { return onLevelUnloaded; } }

		protected virtual void Awake()
		{
			Debug.Log(("Level: " + LevelName + " loaded").Colored(Colors.lightblue));

			levelService = ServiceLocator.GetService<ILevelLoaderService>();
			audioService = ServiceLocator.GetService<IAudioService>();
			uiService = ServiceLocator.GetService<IUIService>();

			if (uiService != null)
			{
				uiService.OnWindowOpened
					.Subscribe(OnWindowOpened)
					.AddTo(disposables);

				uiService.OnWindowClosed
					.Subscribe(OnWindowClosed)
					.AddTo(disposables);;
			}

			levelState = LevelState.Loaded;
			LevelLoaded();
		}

		protected virtual void Start()
		{
			Debug.Log(("Level: " + LevelName + " started").Colored(Colors.lightblue));

			levelState = LevelState.Started;

			onLevelStarted.OnNext(this);
			levelState = LevelState.InProgress;

			if (audioService != null && backgroundMusic != null && backgroundMusic.Clip != null)
				audioService.PlayMusic(backgroundMusic);

			if (uiService != null && displayHUD)
				uiService.OpenHUD();
		}

		protected virtual void LevelLoaded()
		{
			onLevelLoaded.OnNext(this);
		}

		/// <summary>
		/// Completing the level will unload it, and completely remove it from memory, and nextLevel will start loading
		/// </summary>
		public virtual void CompleteLevel()
		{
			Debug.Log(("Level: " + LevelName + " completed").Colored(Colors.lightblue));

			levelState = LevelState.Completed;
			onLevelCompleted.OnNext(this);
		}

		public virtual void Unload()
		{
			if (audioService != null && backgroundMusic != null && backgroundMusic.Clip != null)
				audioService.StopClip(backgroundMusic);

			onLevelUnloaded.OnNext(this);
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
				disposables.Dispose();
			}
		}

		protected virtual void OnDestroy()
		{
			if (hud)
				hud.Close();

			if (audioService != null && backgroundMusic != null && backgroundMusic.Clip != null)
				audioService.StopClip(backgroundMusic);

			onLevelLoaded.Dispose();
			onLevelStarted.Dispose();
			onLevelCompleted.Dispose();
			onLevelUnloaded.Dispose();
		}
	}
}