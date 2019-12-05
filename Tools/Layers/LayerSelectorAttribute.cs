using UnityEngine;

[System.Serializable]
public class LayerSelectorAttribute : PropertyAttribute { }

public class TagSelectorAttribute : PropertyAttribute
{
    public bool UseDefaultTagFieldDrawer = false;
}