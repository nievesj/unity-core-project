using System;
using DG.Tweening;
using UnityEngine;
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

    public struct TransitionParams
    {
        public Canvas Canvas;
        public UIElement UiElement;
        public bool IsOutTransition;
        public RectTransform CanvasRecTransform;
    }

    [System.Serializable]
    public class UIElementTransitionOptions
    {
        public TransitionType transitionType;
        public Ease tweenType;
        public float transitionTime = 0.5f;
        public AudioClip transitionSound;

        private bool _transitionCompleted = false;
        private bool _bypassTransitionTime;

        public void PlayTransition(TransitionParams opts, bool ignoreTransitionTime, Action onComplete)
        {
            var start = Vector2.zero;
            var end = Vector2.zero;
            var rtrans = opts.UiElement.RectTransform;
            _transitionCompleted = false;
            _bypassTransitionTime = ignoreTransitionTime;

            switch (transitionType)
            {
                case TransitionType.Top:
                    start = rtrans.anchoredPosition;
                    end = rtrans.anchoredPosition;
                    end.y -= rtrans.rect.height;

                    if (opts.IsOutTransition)
                    {
                        start = end;
                        end = rtrans.anchoredPosition;
                        end.y += rtrans.rect.height;
                    }

                    Move(rtrans, start, end, onComplete);
                    break;
                case TransitionType.Bottom:
                    start = rtrans.anchoredPosition;
                    end = rtrans.anchoredPosition;
                    end.y += rtrans.rect.height;

                    if (opts.IsOutTransition)
                    {
                        start = end;
                        end = rtrans.anchoredPosition;
                        end.y -= rtrans.rect.height;
                    }

                    Move(rtrans, start, end, onComplete);
                    break;
                case TransitionType.Left:
                    start = rtrans.anchoredPosition;
                    end = rtrans.anchoredPosition;
                    end.x += rtrans.rect.width;

                    if (opts.IsOutTransition)
                    {
                        start = end;
                        end = rtrans.anchoredPosition;
                        end.x -= rtrans.rect.width;
                    }

                    Move(rtrans, start, end, onComplete);
                    break;
                case TransitionType.Right:
                    start = rtrans.anchoredPosition;
                    end = rtrans.anchoredPosition;
                    end.x -= rtrans.rect.width;

                    if (opts.IsOutTransition)
                    {
                        start = end;
                        end = rtrans.anchoredPosition;
                        end.x += rtrans.rect.width;
                    }

                    Move(rtrans, start, end, onComplete);
                    break;
                case TransitionType.Scale:
                    start = Vector2.zero;
                    end = new Vector2(1, 1);

                    if (opts.IsOutTransition)
                    {
                        start = end;
                        end = Vector2.zero;
                    }

                    Scale(rtrans, start, end, onComplete);
                    break;

                case TransitionType.Fade:
                    float fstart = 0;
                    float fend = 1;

                    if (opts.IsOutTransition)
                    {
                        fstart = 1;
                        fend = 0;
                    }

                    Fade(rtrans, fstart, fend, onComplete);
                    break;
            }
        }

        private void Scale(RectTransform transform, Vector2 start, Vector2 end, Action onComplete)
        {
            var ttime = transitionTime;
            if (_bypassTransitionTime)
                ttime = 0;
            
            transform.DOScale(start, 0);
            transform.DOScale(end, ttime)
                .SetEase(tweenType)
                .OnComplete(() =>
                {
                    _transitionCompleted = true;
                    onComplete?.Invoke();
                });
        }

        private void Move(RectTransform transform, Vector2 start, Vector2 end, Action onComplete)
        {
            var ttime = transitionTime;
            if (_bypassTransitionTime)
                ttime = 0;

            transform.DOAnchorPos(end, ttime)
                .SetEase(tweenType)
                .OnComplete(() =>
                {
                    _transitionCompleted = true;
                    onComplete?.Invoke();
                });
        }

        private void Fade(RectTransform transform, float start, float end, Action onComplete)
        {
            var ttime = transitionTime;
            if (_bypassTransitionTime)
                ttime = 0;
            
            var images = transform.GetComponentsInChildren<Image>();
            var completed = 0;

            foreach (var image in images)
            {
                image.DOFade(start, 0);
                image.DOFade(end, ttime)
                    .SetEase(tweenType)
                    .OnComplete(() =>
                    {
                        completed++;
                        if (completed >= images.Length)
                        {
                            _transitionCompleted = true;
                            onComplete?.Invoke();
                        }
                    });
            }
        }
    }
}