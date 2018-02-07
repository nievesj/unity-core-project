using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Services.UI
{
	public enum TransitionType
	{
		NotUsed,
		Scale,
		Left,
		Right,
		Top,
		Bottom,
		Fade
	}

	[System.Serializable]
	public class UIWindowTransitionOptions
	{
		public TransitionType transitionType;
		public LeanTweenType tweenType;
		public float time;

		private UIWindow _window;

		public IObservable<UIWindow> PlayTransition(UIWindow window, bool isOutTransition = false)
		{
			IObservable<UIWindow> ret = null;

			var start = Vector2.zero;
			var end = Vector2.zero;
			var rtrans = window.RTransform;
			_window = window;

			switch (transitionType)
			{
				case TransitionType.Scale:
					start = Vector2.zero;
					end = new Vector2(1, 1);

					if (isOutTransition)
					{
						start = end;
						end = Vector2.zero;
					}

					ret = Scale(rtrans, start, end);
					break;

				case TransitionType.Left:
					start = rtrans.anchoredPosition;
					start.x -= rtrans.rect.width;
					end = rtrans.anchoredPosition;

					if (isOutTransition)
					{
						start = end;
						end = rtrans.anchoredPosition;
						end.x -= rtrans.rect.width;
					}

					ret = Move(rtrans, start, end);
					break;
				case TransitionType.Right:
					start = rtrans.anchoredPosition;
					start.x += rtrans.rect.width;
					end = rtrans.anchoredPosition;

					if (isOutTransition)
					{
						start = end;
						end = rtrans.anchoredPosition;
						end.x += rtrans.rect.width;
					}

					ret = Move(rtrans, start, end);
					break;
				case TransitionType.Top:
					start = rtrans.anchoredPosition;
					start.y += Screen.height;
					end = rtrans.anchoredPosition;

					if (isOutTransition)
					{
						start = end;
						end = rtrans.anchoredPosition;
						end.y += Screen.height;
					}

					ret = Move(rtrans, start, end);
					break;
				case TransitionType.Bottom:
					start = rtrans.anchoredPosition;
					start.y -= Screen.height;
					end = rtrans.anchoredPosition;

					if (isOutTransition)
					{
						start = end;
						end = rtrans.anchoredPosition;
						end.y -= Screen.height;
					}

					ret = Move(rtrans, start, end);
					break;
				case TransitionType.Fade:
					float fstart = 0;
					float fend = 1;

					if (isOutTransition)
					{
						fstart = 1;
						fend = 0;
					}

					ret = Fade(rtrans, fstart, fend);
					break;
			}
			return ret;
		}

		IObservable<UIWindow> Scale(RectTransform window, Vector2 start, Vector2 end)
		{
			var subject = new Subject<UIWindow>();

			LeanTween.scale(window, start, 0);
			LeanTween.scale(window, end, time).setEase(tweenType).setOnComplete(()=>
			{
				subject.OnNext(_window);
				subject.OnCompleted();
			});

			return subject;
		}

		IObservable<UIWindow> Move(RectTransform window, Vector2 start, Vector2 end)
		{
			var subject = new Subject<UIWindow>();

			LeanTween.move(window, start, 0);
			LeanTween.move(window, end, time).setEase(tweenType).setOnComplete(()=>
			{
				subject.OnNext(_window);
				subject.OnCompleted();
			});

			return subject;
		}

		IObservable<UIWindow> Fade(RectTransform window, float start, float end)
		{
			var subject = new Subject<UIWindow>();

			LeanTween.alpha(window, start, 0);
			LeanTween.alpha(window, end, time).setEase(tweenType).setOnComplete(()=>
			{
				subject.OnNext(_window);
				subject.OnCompleted();
			});

			return subject;
		}
	}
}