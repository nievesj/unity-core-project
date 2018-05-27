using Core.Services.UI;
using Core.Services.UpdateManager;
using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Services
{
	public abstract class Service : IInitializable, IDisposable
	{
		public virtual void Initialize() { }
		public virtual void Dispose() { }
	}

	public abstract class CoreBehaviour : MonoBehaviour
	{
		[Inject]
		protected UIService _uiService;

		protected virtual void Start()
		{
			_uiService.OnGamePaused.Subscribe(OnGamePaused);
		}

		protected virtual void Awake() { }
		protected virtual void OnGamePaused(bool isPaused) { }
		protected virtual void OnDestroy() { }
	}
}