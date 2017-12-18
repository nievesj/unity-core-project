using System;
using System.Collections;
using System.Collections.Generic;
using Core.Service;
using Core.Signals;
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

		protected ServiceFramework app;

		protected Signal<IService> serviceConfigured = new Signal<IService>();
		public Signal<IService> ServiceConfigured { get { return serviceConfigured; } }

		protected Signal<IService> serviceStarted = new Signal<IService>();
		public Signal<IService> ServiceStarted { get { return serviceStarted; } }

		protected Signal<IService> serviceStopped = new Signal<IService>();
		public Signal<IService> ServiceStopped { get { return serviceStopped; } }

		public void Configure(ServiceConfiguration config)
		{
			configuration = config as ControlServiceConfiguration;

			serviceStarted.Add(CreateControlObject);
			serviceStopped.Add(DestroyControlObject);

			serviceConfigured.Dispatch(this);
		}

		public void StartService(ServiceFramework application)
		{
			app = application;
			serviceStarted.Dispatch(this);
		}

		public void StopService(ServiceFramework application)
		{
			serviceStopped.Dispatch(this);
		}

		protected void CreateControlObject(IService service)
		{
			GameObject go = GameObject.Instantiate(configuration.MouseTouchControls);
			controls = go.GetComponent<MouseTouchControls>();
			controls.transform.SetParent(app.transform);
		}

		protected void DestroyControlObject(IService service)
		{
			GameObject.Destroy(controls.gameObject);
		}
	}
}