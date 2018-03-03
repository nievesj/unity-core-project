using System.Collections;
using System.Collections.Generic;
using Core.Services.UI;
using Core.Services.UpdateManager;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Services
{
	public abstract class CoreBehaviour : MonoBehaviour
	{
		[Inject]
		protected UpdateService updateService;

		protected virtual void Start()
		{
			//ServiceLocator.GetService<IUIService>().OnGamePaused.Subscribe(OnGamePaused);
			//updateService = ServiceLocator.GetService<IUpdateService>();
		}

		protected virtual void Awake() { }

		protected virtual void OnGamePaused(bool isPaused) { }

		protected virtual void CoreUpdate() { }

		protected virtual void CoreFixedUpdate() { }

		protected virtual void CoreLateUpdate() { }
	}
}