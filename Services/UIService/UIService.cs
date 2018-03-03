using System.Collections;
using System.Collections.Generic;
using Core.Services;
using Core.Services.Assets;
using Core.Services.Factory;
using Core.Services.Levels;
using UniRx;
using UnityEngine;
using Zenject;

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

	public interface IUIService : IService { }

	public class UIService : IService
	{
		[Inject]
		private AssetService _assetService;

		[Inject]
		private FactoryService _factoryService;

		private UIServiceConfiguration _configuration;

		private RectTransform _mainCanvas;

		private UIScreenFader _uiScreenFader;

		private Dictionary<UIElementType, RectTransform> _renderPriorityCanvas;

		private Dictionary<string, UIElement> _activeUIElements;

		//Global signal emited when a window is opened. Hot observable.
		private Subject<UIElement> _onUIElementOpened = new Subject<UIElement>();

		public IObservable<UIElement> OnUIElementOpened { get { return _onUIElementOpened; } }

		//Global signal emited when a window is closed. Hot observable.
		private Subject<UIElement> _onUIElementClosed = new Subject<UIElement>();

		public IObservable<UIElement> OnUIElementClosed { get { return _onUIElementClosed; } }

		//Global signal emited when a game is paused. Hot observable.
		private Subject<bool> _onGamePaused = new Subject<bool>();

		public IObservable<bool> OnGamePaused { get { return _onGamePaused; } }

		public UIService(ServiceConfiguration config)
		{
			_configuration = config as UIServiceConfiguration;

			_activeUIElements = new Dictionary<string, UIElement>();
			_renderPriorityCanvas = new Dictionary<UIElementType, RectTransform>();

			if (_configuration.mainCanvas)
			{
				var canvas = Object.Instantiate<UIContainer>(_configuration.mainCanvas);
				_mainCanvas = canvas.GetComponent<RectTransform>();
				_uiScreenFader = canvas.GetComponentInChildren<UIScreenFader>();
				GameObject.DontDestroyOnLoad(_mainCanvas);

				_renderPriorityCanvas.Add(UIElementType.Dialog, canvas.dialogContainer);
				_renderPriorityCanvas.Add(UIElementType.Panel, canvas.panelContainer);
				_renderPriorityCanvas.Add(UIElementType.Widget, canvas.widgetContainer);
			}
		}

		private void OnGameStart(ServiceLocator locator)
		{
		}

		/// <summary>
		/// Opens a window 
		/// </summary>
		/// <param name="window"> Window name </param>
		/// <returns> Observable </returns>
		public IObservable<UIElement> OpenUIElement(string window)
		{
			BundleRequest bundleNeeded = new BundleRequest(AssetCategoryRoot.Screens, window, window);
			return Observable.Create<UIElement>(
				(IObserver<UIElement> observer) =>
				{
					System.Action<UIElement> OnWindowLoaded = loadedWindow =>
					{
						if (!_mainCanvas)
							observer.OnError(new System.Exception("uiService: StartService - UICanvas is missing from the scene. Was is destroyed?."));

						var obj = _factoryService.Instantiate<UIElement>(loadedWindow, DetermineRenderPriorityCanvas(loadedWindow));

						obj.name = loadedWindow.name;
						obj.OnClosed.Subscribe(UIElementClosed);
						obj.OnOpened.Subscribe(UIElementOpened);

						if (!_activeUIElements.ContainsKey(obj.name))
							_activeUIElements.Add(obj.name, obj);

						//Trigger OnGamePaused signal. Is up to the game to determine what happens. That's beyond the scope of the uiService.
						if (obj is UIDialog)
							_onGamePaused.OnNext(true);

						observer.OnNext(obj);
						observer.OnCompleted();

						Debug.Log(("uiService: Loaded window - " + loadedWindow.name).Colored(Colors.LightBlue));
					};

					return _assetService.GetAndLoadAsset<UIElement>(bundleNeeded).Subscribe(OnWindowLoaded);
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
				return _renderPriorityCanvas[UIElementType.Dialog];
			else if (element is UIPanel)
				return _renderPriorityCanvas[UIElementType.Panel];
			else if (element is UIWidget)
				return _renderPriorityCanvas[UIElementType.Widget];
			else
				return _mainCanvas;
		}

		/// <summary>
		/// Checks if a window is already open 
		/// </summary>
		/// <param name="window"> Window name </param>
		/// <returns> bool </returns>
		public bool IsUIElementOpen(string window)
		{
			return _activeUIElements.ContainsKey(window) ? true : false;
		}

		/// <summary>
		/// Returns the reference of an open window 
		/// </summary>
		/// <param name="window"> Window name </param>
		/// <returns> UIWindow </returns>
		public UIElement GetOpenUIElement(string window)
		{
			return _activeUIElements.ContainsKey(window) ? _activeUIElements[window] : null;
		}

		public IObservable<UIElement> CloseUIElement(UIElement window)
		{
			return window.Close()
				.Subscribe(UIElementClosed) as IObservable<UIElement>;
		}

		public IObservable<UIElement> DarkenScreen(bool block)
		{
			return _uiScreenFader.DarkenScreen(block);
		}

		private void UIElementClosed(UIElement window)
		{
			Debug.Log(("uiService: Closed window - " + window.name).Colored(Colors.LightBlue));

			//Trigger OnGamePaused signal. Is up to the game to determine what happens. That's beyond the scope of the uiService.
			if (window is UIDialog)
				_onGamePaused.OnNext(false);

			_activeUIElements.Remove(window.name);
			_onUIElementClosed.OnNext(window);

			_assetService.UnloadAsset(window.name, true);
			Object.Destroy(window.gameObject);
		}

		private void UIElementOpened(UIElement window)
		{
			_onUIElementOpened.OnNext(window);
		}
	}
}