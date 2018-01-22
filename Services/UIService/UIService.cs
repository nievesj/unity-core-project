using System.Collections;
using System.Collections.Generic;
using Core.Assets;
using Core.Service;
using Core.Signals;
using UniRx;
using UnityEngine;

namespace Core.UI
{
	public interface IUIService : IService
	{
		Signal<UIWindow> OnWindowOpened { get; }
		Signal<UIWindow> OnWindowClosed { get; }

		void Open(UIWindows window);

		void Open(string window);

		void OpenHUD();
	}

	public class UIService : IUIService
	{
		protected UIServiceConfiguration configuration;
		protected Services app;

		protected AssetService assetService;
		protected RectTransform mainCanvas;

		protected Dictionary<string, UIWindow> activeWindows;

		//For the future to pre-load all windows as prefabs. This is needed so theres no waiting time when opening windows and the bundles are remote.
		protected Dictionary<string, UIWindow> preLoadedWindows;

		protected Signal<IService> serviceConfigured = new Signal<IService>();
		public Signal<IService> ServiceConfigured { get { return serviceConfigured; } }

		protected Signal<IService> serviceStarted = new Signal<IService>();
		public Signal<IService> ServiceStarted { get { return serviceStarted; } }

		protected Signal<IService> serviceStopped = new Signal<IService>();
		public Signal<IService> ServiceStopped { get { return serviceStopped; } }

		protected Signal<UIWindow> onWindowOnpened = new Signal<UIWindow>();
		public Signal<UIWindow> OnWindowOpened { get { return onWindowOnpened; } }

		protected Signal<UIWindow> onWindowClosed = new Signal<UIWindow>();
		public Signal<UIWindow> OnWindowClosed { get { return onWindowClosed; } }

		public void Configure(ServiceConfiguration config)
		{
			configuration = config as UIServiceConfiguration;

			serviceConfigured.Dispatch(this);
		}

		public void StartService(Services application)
		{
			app = application;
			serviceStarted.Dispatch(this);

			assetService = Services.GetService<IAssetService>() as AssetService;
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

		public void StopService(Services application)
		{
			serviceStopped.Dispatch(this);
			Object.Destroy(mainCanvas.gameObject);
		}

		public void Open(UIWindows window)
		{
			Open(window.ToString());
		}

		public void Open(string window)
		{
			if (mainCanvas)
				LoadAndActivateWindow(window);
			else
				Debug.LogError("UIService: StartService - Main Canvas is missing.");
		}

		public void OpenHUD()
		{
			Open(configuration.HUD);
		}

		protected void WindowClosed(UIWindow window)
		{
			Debug.Log(("UIService: Closed window - " + window.name).Colored(Colors.lightblue));

			window.Closed.Remove(WindowClosed);
			activeWindows.Remove(window.name);

			onWindowClosed.Dispatch(window);
			assetService.UnloadAsset(window.name, true);

			Object.Destroy(window.gameObject);
		}

		protected void WindowOpened(UIWindow window)
		{
			window.Opened.Remove(WindowOpened);

			if (!activeWindows.ContainsKey(window.name))
				activeWindows.Add(window.name, window);

			onWindowOnpened.Dispatch(window);
		}

		protected void LoadAndActivateWindow(string name)
		{
			BundleNeeded bundleNeeded = new BundleNeeded(AssetCategoryRoot.Windows, name.ToLower(), name.ToLower());
			assetService.GetAndLoadAsset<UIWindow>(bundleNeeded)
				.Subscribe(loadedWindow =>
				{
					Debug.Log(("UIService: Loaded window - " + loadedWindow.name).Colored(Colors.lightblue));

					OnLoadedWindow(loadedWindow as UIWindow);
				});
		}

		protected void OnLoadedWindow(UIWindow window)
		{
			var obj = Object.Instantiate<UIWindow>(window, mainCanvas);

			obj.Closed.Add(WindowClosed);
			obj.Opened.Add(WindowOpened);
			obj.Initialize(this);
		}
	}
}