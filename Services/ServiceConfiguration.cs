using System;
using System.Collections;
using UnityEngine;

namespace Core.Service
{
	public abstract class ServiceConfiguration : ScriptableObject
	{
		protected abstract IService GetServiceClass();

		public IService CreateService()
		{
			IService service = GetServiceClass();
			if (service == null) return null;
			service.Configure(this);
			Services.AddService(name, service);

			return service;
		}
	}
}