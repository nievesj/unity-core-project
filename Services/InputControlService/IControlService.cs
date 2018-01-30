using System;
using System.Collections;
using System.Collections.Generic;
using Core.Service;
using UniRx;
using UnityEngine;

namespace Core.ControlSystem
{
	public interface IControlService : IService
	{
		MouseTouchControls Controls { get; }
	}

	public class ControlService : IControlService
	{
		protected ControlServiceConfiguration configuration;
		protected MouseTouchControls controls;
		public MouseTouchControls Controls { get { return controls; } }

		protected Subject<IService> serviceConfigured = new Subject<IService>();
		public IObservable<IService> ServiceConfigured { get { return serviceConfigured; } }

		protected Subject<IService> serviceStarted = new Subject<IService>();
		public IObservable<IService> ServiceStarted { get { return serviceStarted; } }

		protected Subject<IService> serviceStopped = new Subject<IService>();
		public IObservable<IService> ServiceStopped { get { return serviceStopped; } }

		public void Configure(ServiceConfiguration config)
		{
			configuration = config as ControlServiceConfiguration;

			serviceStarted.Subscribe(CreateControlObject);
			serviceStopped.Subscribe(DestroyControlObject);

			serviceConfigured.OnNext(this);
		}

		public void StartService()
		{
			serviceStarted.OnNext(this);
		}

		public void StopService()
		{
			serviceStopped.OnNext(this);

			serviceConfigured.Dispose();
			serviceStarted.Dispose();
			serviceStopped.Dispose();
		}

		protected void CreateControlObject(IService service)
		{
			GameObject go = GameObject.Instantiate(configuration.MouseTouchControls);
			controls = go.GetComponent<MouseTouchControls>();
			controls.transform.SetParent(ServiceLocator.Instance.transform);
		}

		protected void DestroyControlObject(IService service)
		{
			GameObject.Destroy(controls.gameObject);
		}
	}
}