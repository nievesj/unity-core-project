using Core.Services.Factory;
using UnityEngine;
using Zenject;

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

	public class UpdateService : Service
	{
		[Inject]
		private FactoryService _factoryService;

		protected UpdateServiceConfiguration configuration;

		protected UpdateManager updateManager;

		public UpdateService(ServiceConfiguration config)
		{
			configuration = config as UpdateServiceConfiguration;
		}

		public override void Initialize()
		{
			base.Initialize();

			updateManager = _factoryService.Instantiate<UpdateManager>(configuration.updateManager);
			Object.DontDestroyOnLoad(updateManager);
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