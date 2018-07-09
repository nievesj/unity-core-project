using System.Threading.Tasks;
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

    [System.Serializable]
    public class UIElementTransitionOptions
    {
        public TransitionType transitionType;
        public Ease tweenType;
        public float transitionTime = 0.5f;
        public AudioClip transitionSound;

        private bool _transitionCompleted = false;

        public async Task PlayTransition(UIElement uiElement, bool isOutTransition = false)
        {
            var start = Vector2.zero;
            var end = Vector2.zero;
            var rtrans = uiElement.RectTransform;
            _transitionCompleted = false;

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

                    await Scale(rtrans, start, end);
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

                    await Move(rtrans, start, end);
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

                    await Move(rtrans, start, end);
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

                    await Move(rtrans, start, end);
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

                    await Move(rtrans, start, end);
                    break;
                case TransitionType.Fade:
                    float fstart = 0;
                    float fend = 1;

                    if (isOutTransition)
                    {
                        fstart = 1;
                        fend = 0;
                    }

                    await Fade(rtrans, fstart, fend);
                    break;
            }
        }

        private async Task Scale(RectTransform transform, Vector2 start, Vector2 end)
        {
            transform.DOScale(start, 0);
            transform.DOScale(end, transitionTime)
                .SetEase(tweenType)
                .OnComplete(() => { _transitionCompleted = true; });

            while (!_transitionCompleted)
                await Task.Yield();
        }

        private async Task Move(RectTransform transform, Vector2 start, Vector2 end)
        {
            transform.DOAnchorPos(start, 0);
            transform.DOAnchorPos(end, transitionTime)
                .SetEase(tweenType)
                .OnComplete(() => { _transitionCompleted = true; });

            while (!_transitionCompleted)
                await Task.Yield();
        }

        private async Task Fade(RectTransform transform, float start, float end)
        {
            var images = transform.GetComponentsInChildren<Image>();
            var completed = 0;

            foreach (var image in images)
            {
                image.DOFade(start, 0);
                image.DOFade(end, transitionTime)
                    .SetEase(tweenType)
                    .OnComplete(() =>
                    {
                        completed++;
                        if (completed >= images.Length)
                        {
                            _transitionCompleted = true;
                        }
                    });
            }

            while (!_transitionCompleted)
                await Task.Yield();
        }
    }
}