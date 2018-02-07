using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace Core.Services
{
	public struct ConfigurationServiceName
	{
		public string name;
		public IService service;

		public ConfigurationServiceName(string n, IService svc)
		{
			name = n;
			service = svc;
		}
	}

	public abstract class ServiceConfiguration : ScriptableObject
	{
		abstract protected IService ServiceClass { get; }

		public IObservable<ConfigurationServiceName> CreateService()
		{
			return Observable.Create<ConfigurationServiceName>(
				(IObserver<ConfigurationServiceName> observer)=>
				{
					var subject = new Subject<ConfigurationServiceName>();

					IService service = ServiceClass;
					if (service != null)
					{
						service.Configure(this).Subscribe(s =>
						{
							// IService service = ServiceClass;
							// if (service == null)return null;
							// service.Configure(this);
							// ServiceLocator.AddService(name, service);

							// return service;

							observer.OnNext(new ConfigurationServiceName(name, service));
							observer.OnCompleted();
						});
					}
					else
					{
						observer.OnError(new System.Exception("Failed to create service " + name));
					}

					return subject;
				});
		}
	}
}