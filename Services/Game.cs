using System.Collections;
using System.Collections.Generic;
using Core.Services.Levels;
using Core.Services.Scenes;
using Core.Services.UI;
using UniRx;
using UnityEngine;

namespace Core.Services
{
	/// <summary>
	/// Starting point for _Core framework. This should be treated as the BaseGame.
	/// </summary>
	public class Game : MonoBehaviour
	{
		[SerializeField]
		//Main game configuration. This contains all services to be started when the game starts.
		protected GameConfiguration configuration;
		public GameConfiguration GameConfiguration { get { return configuration; } }

		private static Subject<Game> onGameStarted = new Subject<Game>();
		internal static IObservable<Game> OnGameStarted { get { return onGameStarted; } }

		//Level loader reference. 
		protected ILevelLoaderService LevelLoader { get { return ServiceLocator.GetService<ILevelLoaderService>(); } }

		//Scene loader reference
		protected ISceneLoaderService SceneLoader { get { return ServiceLocator.GetService<ISceneLoaderService>(); } }

		//UIService reference
		protected IUIService UILoader { get { return ServiceLocator.GetService<IUIService>(); } }

		protected virtual void Awake()
		{
			//Make this object persistent
			DontDestroyOnLoad(this.gameObject);

			//Setup service locator, this configures and starts all services.
			ServiceLocator.SetUp(this)
				.DoOnError(e => Debug.LogError("Catastrophic error, services couldn't be created. " + e.Message))
				.Subscribe(OnGameStart);
		}

		/// <summary>
		/// Global signal emitted when the game starts.
		/// </summary>
		/// <param name="locator"></param>
		protected virtual void OnGameStart(ServiceLocator locator)
		{
			UILoader.OnGamePaused.Subscribe(OnGamePaused);
			Debug.Log(("Game Started").Colored(Colors.Lime));

			onGameStarted.OnNext(this);
		}

		protected virtual void OnGamePaused(bool isPaused)
		{
			// if (isPaused)
			// 	Debug.Log(("Game Paused").Colored(Colors.Lime));
			// else
			// 	Debug.Log(("Game Resumed").Colored(Colors.Lime));
		}

		protected virtual void OnDestroy()
		{
			onGameStarted.OnCompleted();
		}
	}
}