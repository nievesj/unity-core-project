using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Services.UI
{
	/// <summary>
	/// A 'Window' is basically a Panel, or any UI element with a canvas renderer.
	/// </summary>
	public class UIWindow : MonoBehaviour
	{
		protected IUIService uiService;

		public UIWindowTransitionOptions inTransition, outTransition;
		public RectTransform RTransform { get { return GetComponent<RectTransform>(); } }

		protected Subject<UIWindow> opened = new Subject<UIWindow>();
		public IObservable<UIWindow> Opened { get { return opened; } }

		protected Subject<UIWindow> closed = new Subject<UIWindow>();
		public IObservable<UIWindow> Closed { get { return closed; } }

		protected Subject<UIWindow> onShow = new Subject<UIWindow>();
		public IObservable<UIWindow> OnShow { get { return onShow; } }

		protected Subject<UIWindow> onHide = new Subject<UIWindow>();
		public IObservable<UIWindow> OnHide { get { return onHide; } }

		protected virtual void Start()
		{
			if (inTransition != null && !inTransition.transitionType.Equals(TransitionType.NotUsed))
				inTransition.PlayTransition(this).Subscribe(OnWindowOpened);
			else
				OnWindowOpened(this);
		}

		public virtual void Initialize(IUIService svc)
		{
			uiService = svc;
		}

		/// <summary>
		/// Close window
		/// </summary>
		/// <returns>Observable</returns>
		public virtual IObservable<UIWindow> Close()
		{
			return Observable.Create<UIWindow>(
				(IObserver<UIWindow> observer)=>
				{
					var subject = new Subject<UIWindow>();

					Action<UIWindow> OnCLosed = window =>
					{
						observer.OnNext(this);
						observer.OnCompleted();

						OnWindowClosed(this);
					};

					if (outTransition != null && !inTransition.transitionType.Equals(TransitionType.NotUsed))
						return outTransition.PlayTransition(this, true).Subscribe(OnCLosed);
					else
						return subject.Subscribe(OnCLosed);
				});
		}

		/// <summary>
		/// Utility method to trigger Close() from a UI event.
		/// </summary>
		public virtual void UIClose()
		{
			Close().Subscribe();;
		}

		protected virtual void OnWindowOpened(UIWindow window)
		{
			opened.OnNext(this);
			opened.OnCompleted();
		}

		protected virtual void OnWindowClosed(UIWindow window)
		{
			closed.OnNext(this);
			closed.OnCompleted();
		}

		protected virtual void OnWindowShow(UIWindow window)
		{
			onShow.OnNext(this);
			onShow.OnCompleted();
		}

		protected virtual void OnWindowHide(UIWindow window)
		{
			onHide.OnNext(this);
			onHide.OnCompleted();
		}

		protected virtual void OnDestroy() {}
	}
}