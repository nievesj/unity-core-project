using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI
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

		public void PlayTransition(UIWindow window, System.Action<UIWindow> OnComplete = null, bool isOutTransition = false)
		{
			var start = Vector2.zero;
			var end = Vector2.zero;
			var rtrans = window.RTransform;

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

					Scale(rtrans, start, end, OnComplete);
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

					Move(rtrans, start, end, OnComplete);
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

					Move(rtrans, start, end, OnComplete);
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

					Move(rtrans, start, end, OnComplete);
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

					Move(rtrans, start, end, OnComplete);
					break;
				case TransitionType.Fade:
					float fstart = 0;
					float fend = 1;

					if (isOutTransition)
					{
						fstart = 1;
						fend = 0;
					}

					Fade(rtrans, fstart, fend, OnComplete);
					break;
			}
		}

		void Scale(RectTransform window, Vector2 start, Vector2 end, System.Action<UIWindow> OnComplete)
		{
			LeanTween.scale(window, start, 0);
			LeanTween.scale(window, end, time).setEase(tweenType).setOnComplete(() =>
			{
				if (OnComplete != null)
					OnComplete(null);
			});
		}

		void Move(RectTransform window, Vector2 start, Vector2 end, System.Action<UIWindow> OnComplete)
		{
			LeanTween.move(window, start, 0);
			LeanTween.move(window, end, time).setEase(tweenType).setOnComplete(() =>
			{
				if (OnComplete != null)
					OnComplete(null);
			});
		}

		void Fade(RectTransform window, float start, float end, System.Action<UIWindow> OnComplete)
		{
			LeanTween.alpha(window, start, 0);
			LeanTween.alpha(window, end, time).setEase(tweenType).setOnComplete(() =>
			{
				if (OnComplete != null)
					OnComplete(null);
			});
		}
	}
}