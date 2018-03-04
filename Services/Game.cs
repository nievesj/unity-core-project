using System.Collections;
using System.Collections.Generic;
using Core.Services.Audio;
using Core.Services.Factory;
using Core.Services.Levels;
using Core.Services.Scenes;
using Core.Services.UI;
using Core.Services.UpdateManager;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Services
{
	/// <summary>
	/// Starting point for _Core framework. This should be treated as the BaseGame.
	/// </summary>
	public abstract class Game : CoreBehaviour
	{
		[Inject]
		protected UIService _UIService;

		[Inject]
		private Subject<Unit> onGameStart;

		protected override void Awake()
		{
			//Make this object persistent
			DontDestroyOnLoad(this.gameObject);

			onGameStart.Subscribe(OnGameStart);
		}

		/// <summary>
		///Global signal emitted when the game starts.
		/// </summary>
		/// <param name="unit"></param>
		protected virtual void OnGameStart(Unit unit)
		{
			_UIService.OnGamePaused.Subscribe(OnGamePaused);
			Debug.Log(("Game Started").Colored(Colors.Lime));
		}
	}
}