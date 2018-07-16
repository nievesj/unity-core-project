using System;
using System.Threading.Tasks;
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

        private readonly Subject<UIElement> _onClosed = new Subject<UIElement>();

        public RectTransform RectTransform => transform as RectTransform;

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

        protected override void Start()
        {
            base.Start();

            Show().Run();
        }

        public IObservable<UIElement> OnClosed()
        {
            return _onClosed;
        }

        /// <summary>
        /// Shows the UI Element and performs any transition 
        /// </summary>
        /// <returns></returns>
        public virtual async Task Show()
        {
            if (inTransition.transitionSound)
                AudioService.PlayClip(inTransition.transitionSound);

            if (inTransition != null && inTransition.transitionType != TransitionType.NotUsed)
                await inTransition.PlayTransition(this);
            else
                OnElementShow();
        }

        /// <summary>
        /// Hides the UI Element after playing the out transition. 
        /// </summary>
        /// <returns></returns>
        public virtual async Task Hide(bool isClose = false)
        {
            if (outTransition.transitionSound)
                AudioService.PlayClip(outTransition.transitionSound);

            if (outTransition != null && outTransition.transitionType != TransitionType.NotUsed)
                await outTransition.PlayTransition(this, true);
            else
                OnElementHide();
        }

        /// <summary>
        /// Close window and tells iservice to destroy the uielement and unload the asset 
        /// </summary>
        /// <returns> Observable </returns>
        public virtual async Task Close()
        {
            await Hide(true);
            
            _onClosed.OnNext(this);
            _onClosed.OnCompleted();
        }
    }
}