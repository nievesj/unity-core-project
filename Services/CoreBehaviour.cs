using System.Collections;
using System.Collections.Generic;
using Core.Services.UI;
using Core.Services.UpdateManager;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Services
{
	public abstract class Service
	{
		internal virtual void SetUp(DiContainer context = null) { }
	}

	public abstract class CoreBehaviour : MonoBehaviour
	{
		[Inject]
		protected UpdateService updateService;

		[Inject]
		protected UIService _uiService;

		protected virtual void Start()
		{
			_uiService.OnGamePaused.Subscribe(OnGamePaused);
		}

		protected virtual void Awake() { }

		protected virtual void OnGamePaused(bool isPaused) { }

		protected virtual void CoreUpdate() { }

		protected virtual void CoreFixedUpdate() { }

		protected virtual void CoreLateUpdate() { }

		protected virtual void OnDestroy() { }
	}
}