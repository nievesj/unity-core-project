using System;
using System.Collections;
using System.Collections.Generic;
using Core.Services;
using UniRx;
using UnityEngine;

namespace Core.Services.Input
{
	public interface IControlService : IService
	{
		MouseTouchControls MouseTouchControls { get; }
	}

	public class ControlService : IControlService
	{
		protected ControlServiceConfiguration configuration;
		protected MouseTouchControls controls;
		public MouseTouchControls MouseTouchControls { get { return controls; } }

		public IObservable<IService> Configure(ServiceConfiguration config)
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();

					configuration = config as ControlServiceConfiguration;
					ServiceLocator.OnGameStart.Subscribe(OnGameStart);

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

					DestroyControlObject();

					observer.OnNext(this);
					return subject.Subscribe();
				});
		}

		protected void OnGameStart(ServiceLocator application)
		{
			CreateControlObject();
		}

		protected void CreateControlObject()
		{
			GameObject go = GameObject.Instantiate(configuration.MouseTouchControls);
			controls = go.GetComponent<MouseTouchControls>();
			controls.transform.SetParent(ServiceLocator.Instance.transform);
		}

		protected void DestroyControlObject()
		{
			GameObject.Destroy(controls.gameObject);
		}
	}
}