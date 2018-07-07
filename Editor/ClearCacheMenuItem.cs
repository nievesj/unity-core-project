using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Basic menu to clear cache
/// </summary>
public class ClearCacheMenuItem
{
	[MenuItem("Core Framework / Clear Asset Bundle Cache")]
	private static void ClearAssetBundleCache()
	{
		//Get all cache paths... in Unity's unique way....
		var cachePaths = new List<string>();
		Caching.GetAllCachePaths(cachePaths); //what?! why?! WHY!?!?

		foreach (var s in cachePaths)
		{
			var cache = Caching.GetCacheByPath(cachePaths[0]);

			Debug.Log(("Cache location: " + s).Colored(Colors.Yellow));
			Debug.Log(("Cache was using: " + ((cache.spaceOccupied / 1024f) / 1024f) + " MB").Colored(Colors.Yellow));

			cache.ClearCache();

			Debug.Log(("Cache cleared.").Colored(Colors.Yellow));
		}

		if (cachePaths.Count < 1)
			Debug.Log(("Cache was empty.").Colored(Colors.Yellow));

		//Delete Application.persistentDataPath
		Directory.Delete(Application.persistentDataPath, true);
		Debug.Log(("Clearing persistent data: Directory " + Application.persistentDataPath + " deleted.").Colored(Colors.Yellow));
	}

	[MenuItem("Core Framework / Enable Simulate Asset Bundles")]
	private static void GetEnableCachedInfo()
	{
		ClearAssetBundleCache();

		EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES = true;
		Debug.Log(("Enabled asset bundle simulation mode.").Colored(Colors.Yellow));
	}

	[MenuItem("Core Framework / Enable Simulate Asset Bundles", true)]
	private static bool GetEnableCachedInfoVal()
	{
		return !EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES ? true : false;
	}

	[MenuItem("Core Framework / Disable Simulate Asset Bundles")]
	private static void GetDisableCachedInfo()
	{
		ClearAssetBundleCache();

		EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES = false;
		Debug.Log(("Disabled asset bundle simulation mode.").Colored(Colors.Yellow));
	}

	[MenuItem("Core Framework / Disable Simulate Asset Bundles", true)]
	private static bool GetDisableCachedInfoVal()
	{
		return EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES ? true : false;
	}
}