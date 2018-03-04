#if UNITY_EDITOR

using UnityEditor;

public static class EditorPreferences
{
	public static bool EDITORPREF_SIMULATE_ASSET_BUNDLES
	{
		get
		{
			return EditorPrefs.GetBool("EditorRefSimulateAssetBundles", false);
		}

		set
		{
			EditorPrefs.SetBool("EditorRefSimulateAssetBundles", value);
		}
	}
}

#endif