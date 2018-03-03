using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Services
{
	/// <summary>
	/// Main accessor for global systems like UI, Asset Bundles, Audio or Game Instance. 
	/// </summary>
	public class ServiceLocator : MonoBehaviour
	{
		//[SerializeField]
		//private static Dictionary<string, IService> _services;

		//private static ServiceLocator _instance;

		////Signal is triggered when all _services are loaded and running.
		//private static Subject<ServiceLocator> _onGameStart = new Subject<ServiceLocator>();

		//internal static IObservable<ServiceLocator> OnGameStart { get { return _onGameStart; } }

		//public static ServiceLocator Instance { get { return _instance; } }

		//private static Game _gameInstance;
		//public static Game GameInstance { get { return _gameInstance; } }

		///// <summary>
		///// Creates and initializes all _services.
		///// </summary>
		///// <param name="game"></param>
		///// <returns></returns>
		//public static IObservable<ServiceLocator> SetUp(Game game)
		//{
		//	return Observable.Create<ServiceLocator>(
		//		(IObserver<ServiceLocator> observer) =>
		//		{
		//			_gameInstance = game;
		//			Instantiate(game);
		//			var subject = new Subject<ServiceLocator>();

		// int servicesCreated = 0; //if (_configuration.disableLogging) //
		// Debug.unityLogger.logEnabled = false;

		// Action<ConfigurationServiceName> onServiceCreated = configServiceName => {
		// servicesCreated++; AddService(configServiceName.name, configServiceName.service);

		// //if (servicesCreated.Equals(_configuration.services.Count)) //{ //
		// Debug.Log(("ServiceLocator: " + _services.Count + " Services created and active").Colored(Colors.Lime));

		// // _onGameStart.OnNext(_instance); _onGameStart.OnCompleted();

		// // observer.OnNext(_instance); // observer.OnCompleted(); //} };

		// Debug.Log(("GameConfiguration: Starting Services").Colored(Colors.Lime)); //foreach (var
		// service in _configuration.services) //{ // Debug.Log(("--- Starting Service: " +
		// service.name).Colored(Colors.Cyan)); //
		// service.CreateService().Subscribe(onServiceCreated); //}

		//			return subject.Subscribe();
		//		});
		//}

		///// <summary>
		///// Gets active service.
		///// </summary>
		///// <returns> Service </returns>
		////public static T GetService<T>() where T : class, IService
		////{
		////	if (!_instance) return null;

		////	if (_services == null) _services = new Dictionary<string, IService>();
		////	foreach (var serviceKVP in _services)
		////		if (serviceKVP.Value is T) return (T)serviceKVP.Value;

		////	return null;
		////}

		////private static void Instantiate(Game game)
		////{
		////	GameObject go = new GameObject(Constants.ServiceLocator);
		////	if (!_instance)
		////		_instance = go.AddComponent<ServiceLocator>();

		////	//_configuration = game.GameConfiguration;
		////	_services = new Dictionary<string, IService>();
		////	DontDestroyOnLoad(_instance.gameObject);
		////}

		////internal static void AddService(string name, IService service)
		////{
		////	if (_services == null) _services = new Dictionary<string, IService>();
		////	if (service == null)
		////	{
		////		throw new System.Exception("Cannot add a null service to the ServiceLocator");
		////	}
		////	_services.Add(name, service);
		////	//service.StartService().Subscribe();
		////}

		////internal static T RemoveService<T>(string serviceName) where T : class, IService
		////{
		////	T returningService = GetService<T>();
		////	if (returningService != null)
		////	{
		////		//_services[serviceName].StopService();
		////		_services.Remove(serviceName);
		////	}
		////	return returningService;
		////}

		//internal static T RemoveService<T>() where T : class, IService
		//{
		//	if (_services == null) _services = new Dictionary<string, IService>();
		//	foreach (var serviceKVP in _services)
		//	{
		//		if (serviceKVP.Value is T)
		//		{
		//			T rtn = (T)serviceKVP.Value;
		//			//_services[serviceKVP.Key].StopService();
		//			_services.Remove(serviceKVP.Key);
		//			return rtn;
		//		}
		//	}
		//	return null;
		//}

		//private void OnDestroy()
		//{
		//	_onGameStart.Dispose();
		//}
	}

	public interface IService
	{
		//IObservable<IService> StartService();

		//IObservable<IService> StopService();

		//IObservable<IService> Configure(ServiceConfiguration config);
	}
}