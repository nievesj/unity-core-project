﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Services.Assets;
using Core.Services.Factory;
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

    public class UIService : Service
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

        public UIService(ServiceConfiguration config)
        {
            _configuration = config as UIServiceConfiguration;

            _activeUIElements = new Dictionary<string, UIElement>();
            _renderPriorityCanvas = new Dictionary<UIElementType, RectTransform>();
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_configuration.mainCanvas)
            {
                var canvas = _factoryService.Instantiate<UIContainer>(_configuration.mainCanvas);

                _mainCanvas = canvas.GetComponent<RectTransform>();
                _uiScreenFader = canvas.GetComponentInChildren<UIScreenFader>();

                UnityEngine.Object.DontDestroyOnLoad(_mainCanvas);

                var canvasElem = canvas.GetComponent<Canvas>();

                if (canvasElem.renderMode == RenderMode.ScreenSpaceCamera)
                    canvasElem.worldCamera = Camera.main;

                _renderPriorityCanvas.Add(UIElementType.Dialog, canvas.dialogContainer);
                _renderPriorityCanvas.Add(UIElementType.Panel, canvas.panelContainer);
                _renderPriorityCanvas.Add(UIElementType.Widget, canvas.widgetContainer);
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
        public async Task<UIElement> OpenUIElement(string window, bool forceLoadFromStreamingAssets = false,
            IProgress<float> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await OpenUIElement<UIElement>(window, forceLoadFromStreamingAssets, progress, cancellationToken);
        }

        /// <summary>
        /// Opens a window
        /// </summary>
        /// <param name="window"> Window name </param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="forceLoadFromStreamingAssets"></param>
        /// <returns> Observable </returns>
        public async Task<T> OpenUIElement<T>(string window, bool forceLoadFromStreamingAssets = false, IProgress<float> progress = null,
            CancellationToken cancellationToken = default(CancellationToken)) where T : UIElement
        {
            var screen = await _assetService.LoadAsset<UIElement>(AssetCategoryRoot.UI, window, forceLoadFromStreamingAssets, progress, cancellationToken);
            if (!_mainCanvas)
                throw new System.Exception("UI Service: StartService - UICanvas is missing from the scene. Was is destroyed?.");

            return await OpenUIElement<T>(screen, progress, cancellationToken);
        }

        public async Task<T> OpenUIElement<T>(UIElement window, IProgress<float> progress = null,
            CancellationToken cancellationToken = default(CancellationToken)) where T : UIElement
        {
            if(cancellationToken.IsCancellationRequested)
                return null;
                        
            var obj = _factoryService.Instantiate<UIElement>(window, DetermineRenderPriorityCanvas(window));
            obj.name = window.name;

            obj.OnClosed().Subscribe(x => { UIElementClosed(x).Run(); });

            if (!_activeUIElements.ContainsKey(obj.name))
                _activeUIElements.Add(obj.name, obj);

            Debug.Log($"UI Service: Loaded window - {obj.name}".Colored(Colors.LightBlue));
            await Awaiters.WaitForEndOfFrame;
            return obj as T;
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

        public IObservable<UIElement> DarkenScreen(bool block)
        {
            return _uiScreenFader.DarkenScreen(block);
        }

        private async Task UIElementClosed(UIElement window)
        {
            Debug.Log(("UI Service: Closed window - " + window.name).Colored(Colors.LightBlue));

            _activeUIElements.Remove(window.name);

            await _assetService.UnloadAsset(window.name, true);
            UnityEngine.Object.Destroy(window.gameObject);
        }
    }
}