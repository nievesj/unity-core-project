using System.Collections;
using System.Collections.Generic;
using Core.Assets;
using Core.Service;
using UniRx;
using UnityEngine;

namespace Core.UI
{
	public interface IUIService : IService
	{
		IObservable<UIWindow> OnWindowOpened { get; }
		IObservable<UIWindow> OnWindowClosed { get; }

		IObservable<UIWindow> OpenWindow(string window);
		IObservable<UIWindow> CloseWindow(UIWindow window);
		bool IsWindowOpen(string window);
		UIWindow GetOpenWindow(string window);
	}

	public class UIService : IUIService
	{
		protected UIServiceConfiguration configuration;
		protected ServiceLocator app;

		protected AssetService assetService;
		protected RectTransform mainCanvas;

		protected Dictionary<string, UIWindow> activeWindows;

		//TODO: Add an option to preload windows as prefabs. This is needed so theres no waiting time when opening windows and the bundles are remote.
		// protected Dictionary<string, UIWindow> preLoadedWindows;

		protected Subject<IService> serviceConfigured = new Subject<IService>();
		public IObservable<IService> ServiceConfigured { get { return serviceConfigured; } }

		protected Subject<IService> serviceStarted = new Subject<IService>();
		public IObservable<IService> ServiceStarted { get { return serviceStarted; } }

		protected Subject<IService> serviceStopped = new Subject<IService>();
		public IObservable<IService> ServiceStopped { get { return serviceStopped; } }

		protected Subject<UIWindow> onWindowOnpened = new Subject<UIWindow>();
		public IObservable<UIWindow> OnWindowOpened { get { return onWindowOnpened; } }

		protected Subject<UIWindow> onWindowClosed = new Subject<UIWindow>();
		public IObservable<UIWindow> OnWindowClosed { get { return onWindowClosed; } }

		public void Configure(ServiceConfiguration config)
		{
			configuration = config as UIServiceConfiguration;

			serviceConfigured.OnNext(this);
			serviceConfigured.OnCompleted();
		}

		public void StartService(ServiceLocator application)
		{
			app = application;

			assetService = ServiceLocator.GetService<IAssetService>()as AssetService;
			activeWindows = new Dictionary<string, UIWindow>();

			//instantiate main canvas
			if (configuration.mainCanvas)
			{
				var canvas = Object.Instantiate<Canvas>(configuration.mainCanvas);
				mainCanvas = canvas.GetComponent<RectTransform>();
				GameObject.DontDestroyOnLoad(mainCanvas);
			}
			else
				serviceStarted.OnError(new System.Exception("UIService: StartService - Main Canvas has not been configured."));

			serviceStarted.OnNext(this);
			serviceStarted.OnCompleted();
		}

		public void StopService(ServiceLocator application)
		{
			serviceStopped.OnNext(this);
			serviceStopped.OnCompleted();

			Object.Destroy(mainCanvas.gameObject);
		}

		public IObservable<UIWindow> OpenWindow(UIWindows window)
		{
			return OpenWindow(window.ToString());
		}

		public IObservable<UIWindow> OpenWindow(string window)
		{
			BundleNeeded bundleNeeded = new BundleNeeded(AssetCategoryRoot.Windows, window.ToLower(), window.ToLower());
			var observable = new Subject<UIWindow>();

			System.Action<UnityEngine.Object> OnWindowLoaded = loadedWindow =>
			{
				if (!mainCanvas)
					observable.OnError(new System.Exception("UIService: StartService - Main Canvas is missing."));

				var obj = Object.Instantiate<UIWindow>(loadedWindow as UIWindow, mainCanvas);

				obj.name = loadedWindow.name;
				obj.Closed.Subscribe(WindowClosed);
				obj.Opened.Subscribe(WindowOpened);
				obj.Initialize(this);

				observable.OnNext(obj);
				observable.OnCompleted();

				Debug.Log(("UIService: Loaded window - " + loadedWindow.name).Colored(Colors.lightblue));
			};

			assetService.GetAndLoadAsset<UIWindow>(bundleNeeded)
				.Subscribe(OnWindowLoaded);

			return observable;
		}

		public bool IsWindowOpen(string window)
		{
			return activeWindows.ContainsKey(window)? true : false;
		}

		public UIWindow GetOpenWindow(string window)
		{
			return activeWindows.ContainsKey(window)? activeWindows[window] : null;
		}

		public IObservable<UIWindow> CloseWindow(UIWindow window)
		{
			return window.Close()
				.Subscribe(WindowClosed)as IObservable<UIWindow>;
		}

		protected void WindowClosed(UIWindow window)
		{
			Debug.Log(("UIService: Closed window - " + window.name).Colored(Colors.lightblue));

			activeWindows.Remove(window.name);
			onWindowClosed.OnNext(window);
			assetService.UnloadAsset(window.name, true);
			Object.Destroy(window.gameObject);
		}

		protected void WindowOpened(UIWindow window)
		{
			if (!activeWindows.ContainsKey(window.name))
				activeWindows.Add(window.name, window);

			onWindowOnpened.OnNext(window);
		}

		protected void OnLoadedWindow(UIWindow window)
		{
			var obj = Object.Instantiate<UIWindow>(window, mainCanvas);

			obj.Closed.Subscribe(WindowClosed);
			obj.Opened.Subscribe(WindowOpened);
			obj.Initialize(this);
		}
	}
}