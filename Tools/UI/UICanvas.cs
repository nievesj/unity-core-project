using UnityEngine;

namespace Core.Tools.UI
{
    public class UICanvas : MonoBehaviour
    {
        [SerializeField]
        internal RectTransform DialogContainer;

        [SerializeField]
        internal RectTransform WidgetContainer;

        [SerializeField]
        internal RectTransform PanelContainer;

        public Canvas MainCanvas { get; private set; }
        public RectTransform RectTransform { get; private set; }

        private void Awake()
        {
            MainCanvas = GetComponent<Canvas>();
            RectTransform = GetComponent<RectTransform>();
        }
    }
}