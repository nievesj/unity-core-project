using Core.Services;
using Core.Services.Assets;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Services.UI
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

		protected Subject<UIWindow> onWindowOnpened = new Subject<UIWindow>();
		public IObservable<UIWindow> OnWindowOpened { get { return onWindowOnpened; } }

		protected Subject<UIWindow> onWindowClosed = new Subject<UIWindow>();
		public IObservable<UIWindow> OnWindowClosed { get { return onWindowClosed; } }

		public IObservable<IService> Configure(ServiceConfiguration config)
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();

					configuration = config as UIServiceConfiguration;
					ServiceLocator.OnGameStart.Subscribe(OnGameStart);

					observer.OnNext(this);
					return subject.Subscribe();
				});
		}

		public IObservable<IService> StartService()
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();

					//instantiate main canvas
					if (configuration.mainCanvas)
					{
						var canvas = Object.Instantiate<Canvas>(configuration.mainCanvas);
						mainCanvas = canvas.GetComponent<RectTransform>();
						GameObject.DontDestroyOnLoad(mainCanvas);
						observer.OnNext(this);

					}
					else
						observer.OnError(new System.Exception("UIService: StartService - Main Canvas has not been configured. Failed to start UI Service."));

					return subject.Subscribe();
				});
		}

		public IObservable<IService> StopService()
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();

					Object.Destroy(mainCanvas.gameObject);

					observer.OnNext(this);
					return subject.Subscribe();
				});
		}

		protected void OnGameStart(ServiceLocator application)
		{
			assetService = ServiceLocator.GetService<IAssetService>();
			activeWindows = new Dictionary<string, UIWindow>();
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

						Debug.Log(("UIService: Loaded window - " + loadedWindow.name).Colored(Colors.LightBlue));
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
			Debug.Log(("UIService: Closed window - " + window.name).Colored(Colors.LightBlue));

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