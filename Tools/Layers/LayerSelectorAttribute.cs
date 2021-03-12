using UnityEngine;

namespace Core.Tools.Layers
{
    [System.Serializable]
    public class LayerSelectorAttribute : PropertyAttribute { }

    public class TagSelectorAttribute : PropertyAttribute
    {
        public bool UseDefaultTagFieldDrawer = false;
    }
}