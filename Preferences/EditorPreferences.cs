#if UNITY_EDITOR

using UnityEditor;

public static class EditorPreferences
{
    public static bool EDITORPREF_SIMULATE_ASSET_BUNDLES 
    { 
        get { return EditorPrefs.GetBool("EditorPrefSimulateAssetBundles", false); } 
        set { EditorPrefs.SetBool("EditorPrefSimulateAssetBundles", value); } 
    }
    
    public static bool EDITORPREF_FIRST_TIME_USE 
    { 
        get { return EditorPrefs.GetBool("EditorPrefFirstTimeUse", true); } 
        set { EditorPrefs.SetBool("EditorPrefFirstTimeUse", value); } 
    }
}

#endif