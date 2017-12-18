using System.Collections;
using System.Collections.Generic;
using Core.Signals;
using UnityEngine;

namespace Core.UI
{
	public class UIWindow : MonoBehaviour
	{
		protected IUIService uiService;

		public UIWindowTransitionOptions inTransition, outTransition;
		public RectTransform RTransform { get { return GetComponent<RectTransform>(); } }

		protected Signal<UIWindow> opened = new Signal<UIWindow>();
		public Signal<UIWindow> Opened { get { return opened; } }

		protected Signal<UIWindow> closed = new Signal<UIWindow>();
		public Signal<UIWindow> Closed { get { return closed; } }

		protected Signal<UIWindow> onShow = new Signal<UIWindow>();
		public Signal<UIWindow> OnShow { get { return onShow; } }

		protected Signal<UIWindow> onHide = new Signal<UIWindow>();
		public Signal<UIWindow> OnHide { get { return onHide; } }

		public virtual void Start()
		{
			if (inTransition != null && !inTransition.transitionType.Equals(TransitionType.NotUsed))
				inTransition.PlayTransition(this, OnWindowOpened);
			else
				OnWindowOpened(this);
		}

		public virtual void Initialize(IUIService svc)
		{
			uiService = svc;
		}

		public virtual void Close()
		{
			if (outTransition != null && !inTransition.transitionType.Equals(TransitionType.NotUsed))
				outTransition.PlayTransition(this, OnWindowClosed, true);
			else
				OnWindowClosed(this);
		}

		public virtual void Show()
		{
			inTransition.PlayTransition(this);
		}

		public virtual void Hide()
		{
			outTransition.PlayTransition(this, null, true);
		}

		protected virtual void OnWindowOpened(UIWindow window)
		{
			opened.Dispatch(this);
		}

		protected virtual void OnWindowClosed(UIWindow window)
		{
			closed.Dispatch(this);
		}

		protected virtual void OnWindowShow(UIWindow window)
		{
			onShow.Dispatch(this);
		}

		protected virtual void OnWindowHide(UIWindow window)
		{
			onHide.Dispatch(this);
		}
	}
}