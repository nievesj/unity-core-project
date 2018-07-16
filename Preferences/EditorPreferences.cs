#if UNITY_EDITOR

using UnityEditor;

public static class EditorPreferences
{
    public static bool EditorprefSimulateAssetBundles 
    { 
        get => EditorPrefs.GetBool("EditorPrefSimulateAssetBundles", false);
        set => EditorPrefs.SetBool("EditorPrefSimulateAssetBundles", value);
    }
    
    public static bool EditorprefFirstTimeUse 
    { 
        get => EditorPrefs.GetBool("EditorPrefFirstTimeUse", true);
        set => EditorPrefs.SetBool("EditorPrefFirstTimeUse", value);
    }
}

#endif