using System;
using System.Collections.Generic;
using Core.Signals;
using UnityEngine;

namespace Core.Service
{
	public class ServiceFramework : MonoBehaviour
	{

		[SerializeField]
		protected GameConfiguration configuration;
		protected Dictionary<string, IService> services;
		protected static ServiceFramework _instance;

		protected static Signal<ServiceFramework> onGameStart = new Signal<ServiceFramework>();
		public static Signal<ServiceFramework> OnGameStart { get { return onGameStart; } }

		protected static Signal<ServiceFramework> onServicesReady = new Signal<ServiceFramework>();
		public static Signal<ServiceFramework> OnServicesReady { get { return onServicesReady; } }

		public static ServiceFramework Instance
		{
			get
			{
				if (_instance == null) Instantiate();
				return _instance;
			}
		}

		public static void Instantiate()
		{
			GameObject go = new GameObject(Constants.ServiceFramework);
			if (!_instance)
				_instance = go.AddComponent<ServiceFramework>();
		}

		protected void Awake()
		{
			if (configuration)
				SetUp();
		}

		public void SetUp(GameConfiguration config)
		{
			configuration = config;
			SetUp();
		}

		protected void SetUp()
		{
			services = new Dictionary<string, IService>();

			DontDestroyOnLoad(this.gameObject);
			configuration.CreateServices(this, OnServicesCreated);
		}

		protected void OnServicesCreated()
		{
			Debug.Log(("ServiceFramework: " + services.Count + " Services created and active").Colored(Colors.lime));
			Debug.Log(("ServiceFramework: Game Started").Colored(Colors.lime));

			onServicesReady.Dispatch(this);
			onGameStart.Dispatch(this);
		}

		/// <summary>
		/// Adds a given service to the ServiceFramework with the name given.
		/// </summary>
		/// <param name="name">The name of the service being added.</param>
		/// <param name="service">The instance of the service being added.</param>
		public void AddService(string name, IService service)
		{
			if (services == null) services = new Dictionary<string, IService>();
			if (service == null)
			{
				throw new System.Exception("cannot add a null service to the ServiceFramework");
			}
			services.Add(name, service);
			service.StartService(this);
		}

		public T GetService<T>(string name) where T : class, IService
		{
			if (services == null) services = new Dictionary<string, IService>();
			if (services.ContainsKey(name))
			{
				if (services[name] is T)
					return (T) services[name];
				else
					Debug.LogWarning("ServiceFramework has a service named \"" + name + "\", but it is type " + name.GetType().ToString() + ", whice is not derived from the specified type " + typeof(T).ToString());
			}
			else
			{
				Debug.LogWarning("ServiceFramework does not have a service named " + name);
			}
			return null;
		}

		public T GetService<T>() where T : class, IService
		{
			if (services == null) services = new Dictionary<string, IService>();
			foreach (var serviceKVP in services)
				if (serviceKVP.Value is T) return (T) serviceKVP.Value;

			return null;
		}

		public void RemoveService(string serviceName)
		{
			if (services == null) return;
			if (services.ContainsKey(serviceName))
			{
				services[serviceName].StopService(this);
				services.Remove(serviceName);
			}
		}

		/// <summary>
		/// Removes the service with the given name and type, and returns it.
		/// </summary>
		/// <typeparam name="T">The type to remove.</typeparam>
		/// <param name="serviceName">The name to seek.</param>
		/// <returns>The service just removed.</returns>
		public T RemoveService<T>(string serviceName) where T : class, IService
		{
			T returningService = GetService<T>(serviceName);
			if (returningService != null)
			{
				services[serviceName].StopService(this);
				services.Remove(serviceName);
			}
			return returningService;
		}

		/// <summary>
		/// Removes the first service matching the given type it can find, and returns it. Slower than finding by name.
		/// </summary>
		/// <typeparam name="T">The type to remove.</typeparam>
		/// <returns>The service just removed.</returns>
		public T RemoveService<T>() where T : class, IService
		{
			if (services == null) services = new Dictionary<string, IService>();
			foreach (var serviceKVP in services)
			{
				if (serviceKVP.Value is T)
				{
					T rtn = (T) serviceKVP.Value;
					services[serviceKVP.Key].StopService(this);
					services.Remove(serviceKVP.Key);
					return rtn;
				}
			}
			return null;
		}
	}

	public interface IService
	{
		Signal<IService> ServiceConfigured { get; }
		Signal<IService> ServiceStarted { get; }
		Signal<IService> ServiceStopped { get; }

		/// <summary>
		/// Starts the service. Called when the service is added to the application.
		/// </summary>
		/// <param name="application">The SAM Application instance.</param>
		void StartService(ServiceFramework application);

		/// <summary>
		/// Stops the service. Called when the service is removed frmo the application.
		/// </summary>
		/// <param name="application">The SAM Application instance.</param>
		void StopService(ServiceFramework application);

		/// <summary>
		/// Configure the service with the specified config. This is called during game startup.
		/// </summary>
		/// <param name="config">The configuration for the service. Will be the specific type for the class.</param>
		void Configure(ServiceConfiguration config);
	}
}