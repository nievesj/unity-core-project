using System;
using System.Linq;
using UniRx;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

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
	public class UIElementTransitionOptions
	{
		public TransitionType transitionType;
		public Ease tweenType;
		public float transitionTime = 0.5f;
		public AudioClip transitionSound;

		private UIElement _uiElement;

		public IObservable<UIElement> PlayTransition(UIElement uiElement, bool isOutTransition = false)
		{
			IObservable<UIElement> ret = null;

			var start = Vector2.zero;
			var end = Vector2.zero;
			var rtrans = uiElement.RectTransform;
			_uiElement = uiElement;

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
					start.y += Screen.height / 2;
					end = rtrans.anchoredPosition;

					if (isOutTransition)
					{
						start = end;
						end = rtrans.anchoredPosition;
						end.y += Screen.height / 2;
					}

					ret = Move(rtrans, start, end);
					break;
				case TransitionType.Bottom:
					start = rtrans.anchoredPosition;
					start.y -= Screen.height / 2;
					end = rtrans.anchoredPosition;

					if (isOutTransition)
					{
						start = end;
						end = rtrans.anchoredPosition;
						end.y -= Screen.height / 2;
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

		IObservable<UIElement> Scale(RectTransform transform, Vector2 start, Vector2 end)
		{
			return Observable.Create<UIElement>(
				observer =>
				{
					var subject = new Subject<UIElement>();

					transform.DOScale(start, 0);
					transform.DOScale(end, transitionTime).SetEase(tweenType)
						.OnComplete(() =>
						{
							observer.OnNext(_uiElement);
							observer.OnCompleted();
						});

					return subject;
				});
		}

		IObservable<UIElement> Move(RectTransform transform, Vector2 start, Vector2 end)
		{
			return Observable.Create<UIElement>(
				observer =>
				{
					var subject = new Subject<UIElement>();

					transform.DOAnchorPos(start, 0);
					transform.DOAnchorPos(end, transitionTime).SetEase(tweenType)
						.OnComplete(() =>
					{
						observer.OnNext(_uiElement);
						observer.OnCompleted();
					});

					return subject;
				});
		}

		IObservable<UIElement> Fade(RectTransform transform, float start, float end)
		{
			return Observable.Create<UIElement>(
				observer =>
				{
					var subject = new Subject<UIElement>();
					var images = transform.GetComponentsInChildren<Image>();
					//TODO add Text too.
					var completed = 0;
					foreach (var image in images)
					{
						image.DOFade(start, 0);
						image.DOFade(image.color.a, transitionTime).SetEase(tweenType)
							.OnComplete(() =>
							{
								completed++;
								if (completed >= images.Length)
								{
									observer.OnNext(_uiElement);
									observer.OnCompleted();
								}
							});
					}

					return subject;
				});
		}
	}
}