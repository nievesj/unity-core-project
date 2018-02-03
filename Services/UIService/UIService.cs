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

		protected IAssetService assetService;
		protected RectTransform mainCanvas;

		protected Dictionary<string, UIWindow> activeWindows;

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

		public void StartService()
		{
			assetService = ServiceLocator.GetService<IAssetService>();
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

		public void StopService()
		{
			serviceStopped.OnNext(this);
			serviceStopped.OnCompleted();

			Object.Destroy(mainCanvas.gameObject);
		}

		/// <summary>
		/// Opens a window
		/// </summary>
		/// <param name="window">Window name</param>
		/// <returns>Observable</returns>
		public IObservable<UIWindow> OpenWindow(string window)
		{
			BundleRequest bundleNeeded = new BundleRequest(AssetCategoryRoot.Windows, window, window);
			return Observable.Create<UIWindow>(
				(IObserver<UIWindow> observer)=>
				{
					System.Action<UIWindow> OnWindowLoaded = loadedWindow =>
					{
						if (!mainCanvas)
							observer.OnError(new System.Exception("UIService: StartService - Main Canvas is missing."));

						var obj = Object.Instantiate<UIWindow>(loadedWindow, mainCanvas);

						obj.name = loadedWindow.name;
						obj.Closed.Subscribe(WindowClosed);
						obj.Opened.Subscribe(WindowOpened);
						obj.Initialize(this);

						if (!activeWindows.ContainsKey(obj.name))
							activeWindows.Add(obj.name, obj);

						observer.OnNext(obj);
						observer.OnCompleted();

						Debug.Log(("UIService: Loaded window - " + loadedWindow.name).Colored(Colors.lightblue));
					};

					return assetService.GetAndLoadAsset<UIWindow>(bundleNeeded).Subscribe(OnWindowLoaded);
				});
		}

		/// <summary>
		/// Checks if a window is already open
		/// </summary>
		/// <param name="window">Window name</param>
		/// <returns>bool</returns>
		public bool IsWindowOpen(string window)
		{
			return activeWindows.ContainsKey(window)? true : false;
		}

		/// <summary>
		/// Returns the reference of an open window
		/// </summary>
		/// <param name="window">Window name</param>
		/// <returns>UIWindow</returns>
		public UIWindow GetOpenWindow(string window)
		{
			return activeWindows.ContainsKey(window)? activeWindows[window] : null;
		}

		/// <summary>
		/// Closes a window
		/// </summary>
		/// <param name="window">Window name</param>
		/// <returns>Observable</returns>
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
			onWindowOnpened.OnNext(window);
		}
	}
}