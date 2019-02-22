using System;
using Core.Services.Audio;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Services.UI
{
    /// <summary>
    /// UIElement is the base class for any UI element that is controlled by the _uiService. 
    /// </summary>
    public abstract class UIElement : CoreBehaviour
    {
        [SerializeField]
        protected bool _PauseGameWhenOpen = false;

        public bool PauseGameWhenOpen => _PauseGameWhenOpen;

        [SerializeField]
        protected UIType _UiType;

        public UIType UIType => _UiType;

        [SerializeField] 
        protected UIElementTransitionOptions inTransition, outTransition;

        [SerializeField]
        protected bool startHidden = true;

        private readonly Subject<UIElement> _onClosed = new Subject<UIElement>();

        public RectTransform RectTransform => transform as RectTransform;

        protected Canvas MainCanvas => UiService.MainCanvas;
        protected TransitionParams TranParams;

        [Inject]
        protected AudioService AudioService;

        /// <summary>
        /// Triggers after the transition on Show ends. 
        /// </summary>
        protected abstract void OnElementShow();

        /// <summary>
        /// Triggers after the transition on Hide ends. 
        /// </summary>
        protected abstract void OnElementHide();

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            TranParams = new TransitionParams
            {
                Canvas = MainCanvas,
                UiElement = this,
                IsOutTransition = false,
                CanvasRecTransform = MainCanvas.GetComponent<RectTransform>()
            };
            
            if(startHidden)
                Hide(true);
        }

        public IObservable<UIElement> OnClosed()
        {
            return _onClosed;
        }

        /// <summary>
        /// Shows the UI Element and performs any transition 
        /// </summary>
        /// <returns></returns>
        public virtual void Show(bool ignoreTransitionTime = false)
        {
            TranParams.IsOutTransition = false;

            if (inTransition.transitionSound)
                AudioService.PlayClip(inTransition.transitionSound);

            if (inTransition != null && inTransition.transitionType != TransitionType.NotUsed)
                inTransition.PlayTransition(TranParams, ignoreTransitionTime, OnElementShow);
        }

        /// <summary>
        /// Hides the UI Element after playing the out transition. 
        /// </summary>
        /// <returns></returns>
        public virtual void Hide(bool ignoreTransitionTime = false)
        {
            TranParams.IsOutTransition = true;

            if (outTransition.transitionSound)
                AudioService.PlayClip(outTransition.transitionSound);

            if (outTransition != null && outTransition.transitionType != TransitionType.NotUsed)
                outTransition.PlayTransition(TranParams, ignoreTransitionTime, OnElementHide);
        }

        /// <summary>
        /// Close window and tells iservice to destroy the uielement and unload the asset 
        /// </summary>
        /// <returns> Observable </returns>
        public virtual void Close()
        {
            Hide(true);

            _onClosed.OnNext(this);
            _onClosed.OnCompleted();
        }
    }
}