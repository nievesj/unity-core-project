using System;
using Core.Services.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Services
{
	/// <summary>
	/// Starting point for Core Framework.
	/// </summary>
	public abstract class Game : CoreBehaviour
	{
		[Inject]
		protected UIService _UIService;

		[Inject]
		private IObservable<Unit> _onGameStarted;

		protected override void Awake()
		{
			//Make this object persistent
			DontDestroyOnLoad(gameObject);
			_onGameStarted.Subscribe(OnGameStart);
		}

		/// <summary>
		///Global signal emitted when the game starts.
		/// </summary>
		/// <param name="unit"></param>
		protected virtual async void OnGameStart(Unit unit)
		{
			_UIService.OnGamePaused().Subscribe(OnGamePaused);
			Debug.Log(("Game Started").Colored(Colors.Lime));
		}
	}
}