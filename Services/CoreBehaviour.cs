using System.Collections;
using System.Collections.Generic;
using Core.Services.UI;
using Core.Services.UpdateManager;
using UniRx;
using UnityEngine;

namespace Core.Services
{
	public class CoreDelegates
	{
		public delegate void OnUpdateDelegate();
	}

	public class BehaviourDelegateType
	{
		public CoreBehaviour behaviour;
		public System.Action method;
		public UpdateType type;

		public BehaviourDelegateType(CoreBehaviour coreBehaviour, System.Action updateDelegate, UpdateType updateType)
		{
			behaviour = coreBehaviour;
			method = updateDelegate;
			type = updateType;
		}
	}

	public class CoreBehaviour : MonoBehaviour
	{
		protected IUpdateService updateService;

		protected virtual void Start()
		{
			ServiceLocator.GetService<IUIService>().OnGamePaused.Subscribe(OnGamePaused);
			updateService = ServiceLocator.GetService<IUpdateService>();
		}

		protected virtual void Awake() {}
		protected virtual void OnGamePaused(bool isPaused) {}
		protected virtual void CoreUpdate() {}
		protected virtual void CoreFixedUpdate() {}
		protected virtual void CoreLateUpdate() {}
	}
}