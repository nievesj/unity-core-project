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

		IObservable<UIWindow> Open(UIWindows window);

		IObservable<UIWindow> Open(string window);

		IObservable<UIWindow> OpenHUD();
	}

	public class UIService : IUIService
	{
		protected UIServiceConfiguration configuration;
		protected ServiceLocator app;

		protected AssetService assetService;
		protected RectTransform mainCanvas;

		protected Dictionary<string, UIWindow> activeWindows;

		//For the future to pre-load all windows as prefabs. This is needed so theres no waiting time when opening windows and the bundles are remote.
		protected Dictionary<string, UIWindow> preLoadedWindows;

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
		}

		public void StartService(ServiceLocator application)
		{
			app = application;
			serviceStarted.OnNext(this);

			assetService = ServiceLocator.GetService<IAssetService>() as AssetService;
			activeWindows = new Dictionary<string, UIWindow>();

			//instantiate main canvas
			if (configuration.mainCanvas)
			{
				var canvas = Object.Instantiate<Canvas>(configuration.mainCanvas);
				mainCanvas = canvas.GetComponent<RectTransform>();
				GameObject.DontDestroyOnLoad(mainCanvas);
			}
			else
				Debug.LogError("UIService: StartService - Main Canvas has not been configured.");
		}

		public void StopService(ServiceLocator application)
		{
			serviceStopped.OnNext(this);

			serviceConfigured.Dispose();
			serviceStarted.Dispose();
			serviceStopped.Dispose();
			onWindowOnpened.Dispose();
			onWindowClosed.Dispose();

			Object.Destroy(mainCanvas.gameObject);
		}

		public IObservable<UIWindow> Open(UIWindows window)
		{
			return Open(window.ToString());
		}

		public IObservable<UIWindow> Open(string window)
		{
			BundleNeeded bundleNeeded = new BundleNeeded(AssetCategoryRoot.Windows, window.ToLower(), window.ToLower());
			var ret = assetService.GetAndLoadAsset<UIWindow>(bundleNeeded)
				.Subscribe(loadedWindow =>
				{
					if (mainCanvas)
					{
						var obj = Object.Instantiate<UIWindow>(loadedWindow as UIWindow, mainCanvas);

						obj.Closed.Subscribe(WindowClosed);
						obj.Opened.Subscribe(WindowOpened);
						obj.Initialize(this);
						Debug.Log(("UIService: Loaded window - " + loadedWindow.name).Colored(Colors.lightblue));
					}
					else
					{
						Debug.LogError("UIService: StartService - Main Canvas is missing.");
					}
				});

			return ret as IObservable<UIWindow>;
		}

		public IObservable<UIWindow> OpenHUD()
		{
			return Open(configuration.HUD);
		}

		protected void WindowClosed(UIWindow window)
		{
			Debug.Log(("UIService: Closed window - " + window.name).Colored(Colors.lightblue));

			// window.Closed.Remove(WindowClosed);
			activeWindows.Remove(window.name);
			onWindowClosed.OnNext(window);
			assetService.UnloadAsset(window.name, true);
			Object.Destroy(window.gameObject);
		}

		protected void WindowOpened(UIWindow window)
		{
			// window.Opened.Remove(WindowOpened);

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