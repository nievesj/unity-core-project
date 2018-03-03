using System.Collections;
using System.Collections.Generic;
using Core.Services.Levels;
using Core.Services.Scenes;
using Core.Services.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Services
{
	/// <summary>
	/// Starting point for _Core framework. This should be treated as the BaseGame. 
	/// </summary>
	public class Game : CoreBehaviour
	{
		private static Subject<Game> onGameStarted = new Subject<Game>();

		internal static IObservable<Game> OnGameStarted { get { return onGameStarted; } }

		//Level loader reference.
		[Inject]
		protected LevelLoaderService LevelLoader;

		//Scene loader reference
		[Inject]
		protected SceneLoaderService SceneLoader;

		//uiService reference
		[Inject]
		protected UIService UILoader;

		protected override void Awake()
		{
			//Make this object persistent
			DontDestroyOnLoad(this.gameObject);

			//Setup service locator, this configures and starts all services.
			//ServiceLocator.SetUp(this)
			//	.DoOnError(e => Debug.LogError("Catastrophic error, services couldn't be created. " + e.Message))
			//	.Subscribe(OnGameStart);
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

		protected virtual void OnDestroy()
		{
			onGameStarted.OnCompleted();
		}
	}
}