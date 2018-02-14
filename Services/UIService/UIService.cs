using Core.Services;
using Core.Services.Assets;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Services.UI
{
	/// <summary>
	/// Used to determine the parent of the UI Element
	/// </summary>
	public enum UIElementType
	{
		Dialog,
		Widget,
		Panel
	}

	public interface IUIService : IService
	{
		IObservable<UIElement> OnUIElementOpened { get; }
		IObservable<UIElement> OnUIElementClosed { get; }
		IObservable<bool> OnGamePaused { get; }

		IObservable<UIElement> OpenUIElement(string window);
		IObservable<UIElement> CloseUIElement(UIElement window);
		bool IsUIElementOpen(string window);
		UIElement GetOpenUIElement(string window);
	}

	public class UIService : IUIService
	{
		protected UIServiceConfiguration configuration;

		protected IAssetService assetService;
		protected RectTransform mainCanvas;
		protected Dictionary<UIElementType, RectTransform> renderPriorityCanvas;

		protected Dictionary<string, UIElement> activeUIElements;

		//Global signal emited when a window is opened. Hot observable.
		protected Subject<UIElement> onUIElementOpened = new Subject<UIElement>();
		public IObservable<UIElement> OnUIElementOpened { get { return onUIElementOpened; } }

		//Global signal emited when a window is closed. Hot observable.
		protected Subject<UIElement> onUIElementClosed = new Subject<UIElement>();
		public IObservable<UIElement> OnUIElementClosed { get { return onUIElementClosed; } }

		//Global signal emited when a game is paused. Hot observable.
		protected Subject<bool> onGamePaused = new Subject<bool>();
		public IObservable<bool> OnGamePaused { get { return onGamePaused; } }

		public IObservable<IService> Configure(ServiceConfiguration config)
		{
			return Observable.Create<IService>(
				(IObserver<IService> observer)=>
				{
					var subject = new Subject<IService>();

					configuration = config as UIServiceConfiguration;

					activeUIElements = new Dictionary<string, UIElement>();
					renderPriorityCanvas = new Dictionary<UIElementType, RectTransform>();

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
						var canvas = Object.Instantiate<UIContainer>(configuration.mainCanvas);
						mainCanvas = canvas.GetComponent<RectTransform>();
						GameObject.DontDestroyOnLoad(mainCanvas);

						renderPriorityCanvas.Add(UIElementType.Dialog, canvas.dialogContainer);
						renderPriorityCanvas.Add(UIElementType.Panel, canvas.panelContainer);
						renderPriorityCanvas.Add(UIElementType.Widget, canvas.widgetContainer);

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

					onUIElementOpened.Dispose();
					onUIElementClosed.Dispose();
					onGamePaused.Dispose();

					observer.OnNext(this);
					return subject.Subscribe();
				});
		}

		protected void OnGameStart(ServiceLocator application)
		{
			assetService = ServiceLocator.GetService<IAssetService>();
		}

		/// <summary>
		/// Opens a window
		/// </summary>
		/// <param name="window">Window name</param>
		/// <returns>Observable</returns>
		public IObservable<UIElement> OpenUIElement(string window)
		{
			BundleRequest bundleNeeded = new BundleRequest(AssetCategoryRoot.Windows, window, window);
			return Observable.Create<UIElement>(
				(IObserver<UIElement> observer)=>
				{
					System.Action<UIElement> OnWindowLoaded = loadedWindow =>
					{
						if (!mainCanvas)
							observer.OnError(new System.Exception("UIService: StartService - UICanvas is missing from the scene. Was is destroyed?."));

						var obj = Object.Instantiate<UIElement>(loadedWindow, DetermineRenderPriorityCanvas(loadedWindow));

						obj.name = loadedWindow.name;
						obj.OnClosed.Subscribe(UIElementClosed);
						obj.OnOpened.Subscribe(UIElementOpened);
						obj.Initialize(this);

						if (!activeUIElements.ContainsKey(obj.name))
							activeUIElements.Add(obj.name, obj);

						//Trigger OnGamePaused signal. Is up to the game to determine what happens. That's beyond the scope of the UIService.
						if (obj is UIDialog)
							onGamePaused.OnNext(true);

						observer.OnNext(obj);
						observer.OnCompleted();

						Debug.Log(("UIService: Loaded window - " + loadedWindow.name).Colored(Colors.LightBlue));
					};

					return assetService.GetAndLoadAsset<UIElement>(bundleNeeded).Subscribe(OnWindowLoaded);
				});
		}

		/// <summary>
		/// Method determines the render priority of each UI element instance.
		/// Dialogs: draw on top of everything
		/// Widget: Draw on tops of Panels but below Dialogs
		/// Panel: Lowest render priority
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		private RectTransform DetermineRenderPriorityCanvas(UIElement element)
		{
			if (element is UIDialog)
				return renderPriorityCanvas[UIElementType.Dialog];

			else if (element is UIPanel)
				return renderPriorityCanvas[UIElementType.Panel];

			else if (element is UIWidget)
				return renderPriorityCanvas[UIElementType.Widget];

			else
				return mainCanvas;
		}

		/// <summary>
		/// Checks if a window is already open
		/// </summary>
		/// <param name="window">Window name</param>
		/// <returns>bool</returns>
		public bool IsUIElementOpen(string window)
		{
			return activeUIElements.ContainsKey(window)? true : false;
		}

		/// <summary>
		/// Returns the reference of an open window
		/// </summary>
		/// <param name="window">Window name</param>
		/// <returns>UIWindow</returns>
		public UIElement GetOpenUIElement(string window)
		{
			return activeUIElements.ContainsKey(window)? activeUIElements[window] : null;
		}

		/// <summary>
		/// Closes a window
		/// </summary>
		/// <param name="window">Window name</param>
		/// <returns>Observable</returns>
		public IObservable<UIElement> CloseUIElement(UIElement window)
		{
			return window.Close()
				.Subscribe(UIElementClosed)as IObservable<UIElement>;
		}

		protected void UIElementClosed(UIElement window)
		{
			Debug.Log(("UIService: Closed window - " + window.name).Colored(Colors.LightBlue));

			//Trigger OnGamePaused signal. Is up to the game to determine what happens. That's beyond the scope of the UIService.
			if (window is UIDialog)
				onGamePaused.OnNext(false);

			activeUIElements.Remove(window.name);
			onUIElementClosed.OnNext(window);

			assetService.UnloadAsset(window.name, true);
			Object.Destroy(window.gameObject);
		}

		protected void UIElementOpened(UIElement window)
		{
			onUIElementOpened.OnNext(window);
		}
	}
}