using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Services.UpdateManager
{
	public enum UpdateType
	{
		Update,
		FidexUpdate,
		LateUpdate
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

	public interface IUpdateService : IService
	{
		void Attach(BehaviourDelegateType behaviourDelegateType);
		void Detach(BehaviourDelegateType behaviourDelegateType);
	}

	public class UpdateService : IUpdateService
	{
		protected UpdateServiceConfiguration configuration;
		protected UpdateManager updateManager;

		public IObservable<IService> Configure(ServiceConfiguration config)
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();
					ServiceLocator.OnGameStart.Subscribe(OnGameStart);
					configuration = config as UpdateServiceConfiguration;

					observer.OnNext(this);
					return subject.Subscribe();
				});
		}

		public IObservable<IService> StartService()
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();
					updateManager = Object.Instantiate<UpdateManager>(configuration.updateManager);

					GameObject.DontDestroyOnLoad(updateManager);

					observer.OnNext(this);
					return subject.Subscribe();
				});
		}

		public IObservable<IService> StopService()
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();

					observer.OnNext(this);
					return subject.Subscribe();
				});
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