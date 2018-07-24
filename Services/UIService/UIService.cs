using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Services.Assets;
using Core.Services.Factory;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Zenject;

namespace Core.Services.UI
{
    /// <summary>
    /// Used to determine the parent of the UI Element
    /// </summary>
    public enum UIType
    {
        Panel,
        Dialog,
        Widget,
        ScreenBlocker
    }
    
    public class UIService : Service
    {
        [Inject]
        private AssetService _assetService;

        [Inject]
        private FactoryService _factoryService;

        private readonly UIServiceConfiguration _configuration;

        private RectTransform _mainCanvas;

        private UIScreenBlocker _uiScreenBlocker;

        private readonly Dictionary<UIType, RectTransform> _renderPriorityCanvas;

        private readonly Dictionary<string, UIElement> _activeUIElements;
        
        private readonly Subject<bool> _onGamePaused = new Subject<bool>();

        public UIService(ServiceConfiguration config)
        {
            _configuration = config as UIServiceConfiguration;

            _activeUIElements = new Dictionary<string, UIElement>();
            _renderPriorityCanvas = new Dictionary<UIType, RectTransform>();
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_configuration.MainCanvas)
            {
                var canvas = _factoryService.Instantiate(_configuration.MainCanvas);

                _mainCanvas = canvas.GetComponent<RectTransform>();
                _uiScreenBlocker = _factoryService.Instantiate(_configuration.UIScreenBlocker,_mainCanvas.transform);
                
                UnityEngine.Object.DontDestroyOnLoad(_mainCanvas);

                var canvasElem = canvas.GetComponent<Canvas>();

                if (canvasElem.renderMode == RenderMode.ScreenSpaceCamera)
                    canvasElem.worldCamera = Camera.main;

                _renderPriorityCanvas.Add(UIType.Dialog, canvas.DialogContainer);
                _renderPriorityCanvas.Add(UIType.Panel, canvas.PanelContainer);
                _renderPriorityCanvas.Add(UIType.Widget, canvas.WidgetContainer);
            }
        }

        /// <summary>
        /// Opens a window
        /// </summary>
        /// <param name="window"> Window name </param>
        /// <param name="forceLoadFromStreamingAssets"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns> Observable </returns>
        public async Task<UIElement> OpenUI(string window, bool forceLoadFromStreamingAssets = false,
            IProgress<float> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await OpenUI<UIElement>(window, forceLoadFromStreamingAssets, progress, cancellationToken);
        }

        /// <summary>
        /// Opens a window
        /// </summary>
        /// <param name="window"> Window name </param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="forceLoadFromStreamingAssets"></param>
        /// <returns> Observable </returns>
        public async Task<T> OpenUI<T>(string window, bool forceLoadFromStreamingAssets = false, IProgress<float> progress = null,
            CancellationToken cancellationToken = default(CancellationToken)) where T : UIElement
        {
            var screen = await _assetService.LoadAsset<UIElement>(AssetCategoryRoot.UI, window, forceLoadFromStreamingAssets, progress, cancellationToken);
            if (!_mainCanvas)
                throw new Exception("UI Service: StartService - UICanvas is missing from the scene. Was is destroyed?.");

            return await OpenUI<T>(screen, progress, cancellationToken);
        }

        public async Task<T> OpenUI<T>(UIElement window, IProgress<float> progress = null,
            CancellationToken cancellationToken = default(CancellationToken)) where T : UIElement
        {
            if(cancellationToken.IsCancellationRequested)
                return null;
                        
            var obj = _factoryService.Instantiate(window, DetermineRenderPriorityCanvas(window.UIType));
            obj.name = window.name;

            obj.OnClosed().Subscribe(x => { UIElementClosed(x).Run(); });

            if (!_activeUIElements.ContainsKey(obj.name))
                _activeUIElements.Add(obj.name, obj);
            
            if(obj.PauseGameWhenOpen)
                PauseResume(true);

            Debug.Log($"UI Service: Loaded window - {obj.name}".Colored(Colors.LightBlue));
            await UniTask.Yield(cancellationToken: cancellationToken);
            return obj as T;
        }

        /// <summary>
        /// Method determines the render priority of each UI element instance.
        /// Dialogs: draw on top of everything
        /// Widget: Draw on tops of Panels but below Dialogs
        /// Panel: Lowest render priority
        /// </summary>
        /// <returns></returns>
        private RectTransform DetermineRenderPriorityCanvas(UIType uiType)
        {
            switch (uiType)
            {
                case UIType.Panel:
                    return _renderPriorityCanvas[UIType.Panel];
                    break;
                case UIType.Dialog:
                    return _renderPriorityCanvas[UIType.Dialog];
                    break;
                case UIType.Widget:
                    return _renderPriorityCanvas[UIType.Widget];
                    break;
                default:
                    return _mainCanvas;
            }
        }

        /// <summary>
        /// Checks if a window is already open
        /// </summary>
        /// <param name="window"> Window name </param>
        /// <returns> bool </returns>
        public bool IsUIElementOpen(string window)
        {
            var win = GetOpenUIElement(window);
            return win != null;
        }

        /// <summary>
        /// Checks if a window is already open
        /// </summary>
        /// <param name="window"> Window name </param>
        /// <returns> bool </returns>
        public bool IsUIElementOpen<T>() where T : UIElement
        {
            var window = GetOpenUIElement<T>();
            return window != null;
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

        /// <summary>
        /// Returns the reference of an open window
        /// </summary>
        /// <param name="window"> Window name </param>
        /// <returns> UIWindow </returns>
        public UIElement GetOpenUIElement<T>() where T : UIElement
        {
            foreach (var value in _activeUIElements.Values)
            {
                if (value is T)
                    return value as T;
            }

            return null;
        }

        public async Task BlockScreen(bool block)
        {
            await _uiScreenBlocker.BlockScreen(block);
        }

        public IObservable<bool> OnGamePaused()
        {
            return _onGamePaused;
        }

        private void PauseResume(bool isPause)
        {
            _onGamePaused.OnNext(isPause);
        }

        private async Task UIElementClosed(UIElement window)
        {
            Debug.Log(("UI Service: Closed window - " + window.name).Colored(Colors.LightBlue));

            _activeUIElements.Remove(window.name);

            await _assetService.UnloadAsset(window.name, true);

            if (window.PauseGameWhenOpen)
                PauseResume(false);
            
            UnityEngine.Object.Destroy(window.gameObject);
        }
    }
}