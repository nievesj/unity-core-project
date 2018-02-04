using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class EditorPreferences
{
	//FIXME: This class should be inside an Editor folder. 
	public static bool EDITORPREF_SIMULATE_ASSET_BUNDLES
	{
		get
		{
#if UNITY_EDITOR
			return EditorPrefs.GetBool("EditorRefSimulateAssetBundles", false);
#else
			return false;
#endif

		}
#if UNITY_EDITOR

		set
		{
			EditorPrefs.SetBool("EditorRefSimulateAssetBundles", value);
		}
#endif
	}

}