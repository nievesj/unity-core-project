using System;
using System.Collections;
using UnityEngine;

namespace Core.Service
{
	public abstract class ServiceConfiguration : ScriptableObject
	{
		abstract protected IService ServiceClass { get; }

		public IService CreateService()
		{
			IService service = ServiceClass;
			if (service == null) return null;
			service.Configure(this);
			ServiceLocator.AddService(name, service);

			return service;
		}
	}
}