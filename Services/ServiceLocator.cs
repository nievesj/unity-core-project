using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Service
{
	/// <summary>
	/// Service Locator.
	/// </summary>
	public class ServiceLocator : MonoBehaviour
	{
		[SerializeField]
		private static GameConfiguration configuration;
		private static Dictionary<string, IService> services;
		private static ServiceLocator _instance;

		private static Subject<ServiceLocator> onGameStart = new Subject<ServiceLocator>();
		public static IObservable<ServiceLocator> OnGameStart { get { return onGameStart; } }

		public static ServiceLocator Instance { get { return _instance; } }

		private static void Instantiate(Transform parent)
		{
			GameObject go = new GameObject(Constants.ServiceLocator);
			if (!_instance)
				_instance = go.AddComponent<ServiceLocator>();
		}

		public static void SetUp(Game game)
		{
			Instantiate(game.transform);

			configuration = game.GameConfiguration;
			services = new Dictionary<string, IService>();

			DontDestroyOnLoad(_instance.gameObject);

			CreateServices()
				.Subscribe(allServicesCreated =>
				{
					if (allServicesCreated)
					{
						Debug.Log(("ServiceLocator: " + services.Count + " Services created and active").Colored(Colors.lime));
						Debug.Log(("ServiceLocator: Game Started").Colored(Colors.lime));

						onGameStart.OnNext(_instance);
						onGameStart.OnCompleted();
					}
				});
		}

		private static IObservable<bool> CreateServices()
		{
			var subject = new ReplaySubject<bool>();

			int servicesCreated = 0;
			if (configuration.disableLogging)
				Debug.unityLogger.logEnabled = false;

			Debug.Log(("GameConfiguration: Starting Services").Colored(Colors.lime));
			foreach (var service in configuration.services)
			{
				Debug.Log(("--- Starting Service: " + service.name).Colored(Colors.cyan));
				service.CreateService();

				servicesCreated++;
			}

			if (servicesCreated.Equals(configuration.services.Count))
			{
				subject.OnNext(true);
				subject.OnCompleted();
			}
			else
				subject.OnError(new System.Exception("Services.CreateServices | Failed to create services. Fatal error."));

			return subject;
		}

		public static void AddService(string name, IService service)
		{
			if (services == null) services = new Dictionary<string, IService>();
			if (service == null)
			{
				throw new System.Exception("cannot add a null service to the ServiceFramework");
			}
			services.Add(name, service);
			service.StartService(_instance);
		}

		public static T GetService<T>() where T : class, IService
		{
			if (!_instance) return null;

			if (services == null) services = new Dictionary<string, IService>();
			foreach (var serviceKVP in services)
				if (serviceKVP.Value is T) return (T) serviceKVP.Value;

			return null;
		}

		public static T RemoveService<T>(string serviceName) where T : class, IService
		{
			T returningService = GetService<T>();
			if (returningService != null)
			{
				services[serviceName].StopService(_instance);
				services.Remove(serviceName);
			}
			return returningService;
		}

		public static T RemoveService<T>() where T : class, IService
		{
			if (services == null) services = new Dictionary<string, IService>();
			foreach (var serviceKVP in services)
			{
				if (serviceKVP.Value is T)
				{
					T rtn = (T) serviceKVP.Value;
					services[serviceKVP.Key].StopService(_instance);
					services.Remove(serviceKVP.Key);
					return rtn;
				}
			}
			return null;
		}

		private void OnDestroy()
		{
			onGameStart.Dispose();
		}
	}

	public interface IService
	{
		IObservable<IService> ServiceConfigured { get; }
		IObservable<IService> ServiceStarted { get; }
		IObservable<IService> ServiceStopped { get; }

		void StartService(ServiceLocator application);

		void StopService(ServiceLocator application);

		void Configure(ServiceConfiguration config);
	}
}