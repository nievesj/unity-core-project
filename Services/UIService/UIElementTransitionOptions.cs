using System;
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
    public class UIElementTransitionOptions
    {
        public TransitionType transitionType;
        public LeanTweenType tweenType;
        public float transitionTime = 0.3f;
        public AudioClip transitionSound;

        private UIElement _uiElement;

        public IObservable<UIElement> PlayTransition(UIElement uiElement, bool isOutTransition = false)
        {
            IObservable<UIElement> ret = null;

            var start = Vector2.zero;
            var end = Vector2.zero;
            var rtrans = uiElement.RTransform;
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

        private IObservable<UIElement> Scale(RectTransform transform, Vector2 start, Vector2 end)
        {
            return Observable.Create<UIElement>(
                observer =>
                {
                    LeanTween.scale(transform, start, 0);
                    LeanTween.scale(transform, end, transitionTime)
                        .setEase(tweenType)
                        .setOnComplete(() =>
                        {
                            observer.OnNext(_uiElement);
                            observer.OnCompleted();
                        });

                    return Disposable.Empty;
                });
        }

        private IObservable<UIElement> Move(RectTransform transform, Vector2 start, Vector2 end)
        {
            return Observable.Create<UIElement>(
                observer =>
                {
                    LeanTween.move(transform, start, 0);
                    LeanTween.move(transform, end, transitionTime)
                        .setEase(tweenType)
                        .setOnComplete(() =>
                        {
                            observer.OnNext(_uiElement);
                            observer.OnCompleted();
                        });

                    return Disposable.Empty;
                });
        }

        private IObservable<UIElement> Fade(RectTransform transform, float start, float end)
        {
            return Observable.Create<UIElement>(
                observer =>
                {
                    LeanTween.alpha(transform, start, 0);
                    LeanTween.alpha(transform, end, transitionTime)
                        .setEase(tweenType)
                        .setOnComplete(() =>
                        {
                            observer.OnNext(_uiElement);
                            observer.OnCompleted();
                        });

                    return Disposable.Empty;
                });
        }
    }
}