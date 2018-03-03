using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Services.UpdateManager
{
	public enum UpdateType
	{
		Update,
		FixedUpdate,
		LateUpdate
	}

	public class BehaviourDelegateType
	{
		public CoreBehaviour behaviour;
		public System.Action updateMethod;
		public UpdateType type;

		public BehaviourDelegateType(CoreBehaviour coreBehaviour, System.Action updateDelegate, UpdateType updateType)
		{
			behaviour = coreBehaviour;
			updateMethod = updateDelegate;
			type = updateType;
		}
	}

	public interface IUpdateService : IService
	{
		void Attach(BehaviourDelegateType behaviourDelegateType);

		void Detach(BehaviourDelegateType behaviourDelegateType);
	}

	public class UpdateService : IUpdateService
	{
		protected UpdateServiceConfiguration configuration;
		protected UpdateManager updateManager;

		public UpdateService(ServiceConfiguration config)
		{
			ServiceLocator.OnGameStart.Subscribe(OnGameStart);
			configuration = config as UpdateServiceConfiguration;

			updateManager = Object.Instantiate<UpdateManager>(configuration.updateManager);

			GameObject.DontDestroyOnLoad(updateManager);
		}

		protected void OnGameStart(ServiceLocator locator)
		{
			//TODO: add pause, this should notify updateManager to pause
		}

		public void Attach(BehaviourDelegateType behaviourDelegateType)
		{
			if (updateManager)
				updateManager.Attach(behaviourDelegateType);
		}

		public void Detach(BehaviourDelegateType behaviourDelegateType)
		{
			if (updateManager)
				updateManager.Detach(behaviourDelegateType);
		}
	}
}