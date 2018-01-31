using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEditor;
using UnityEngine;

public class ClearCacheMenuItem
{
	[MenuItem("_Core / Clear Asset Bundle Cache")]
	private static void CreateRedBlueGameObject()
	{
		var cachePaths = new List<string>();
		Caching.GetAllCachePaths(cachePaths); //what?! why?! WHY!?!?

		foreach (var s in cachePaths)
		{
			var cache = Caching.GetCacheByPath(cachePaths[0]);

			Debug.Log(("Cache location: " + s).Colored(Colors.yellow));
			Debug.Log(("Cache was using: " + ((cache.spaceOccupied / 1024f)/ 1024f)+ " MB").Colored(Colors.yellow));

			cache.ClearCache();

			Debug.Log(("Cache cleared.").Colored(Colors.yellow));
		}

		if (cachePaths.Count < 1)
		{
			Debug.Log(("Cache was empty.").Colored(Colors.yellow));
		}

		Directory.Delete(Application.persistentDataPath, true);
		Debug.Log(("Clearing persistent data: Directory " + Application.persistentDataPath + " deleted.").Colored(Colors.yellow));
	}

	[MenuItem("_Core / Enable Simulate Asset Bundles")]
	private static void GetEnableCachedInfo()
	{
		CreateRedBlueGameObject();

		EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES = true;
		Debug.Log(("Enabled asset bundle simulation mode.").Colored(Colors.yellow));
	}

	[MenuItem("_Core / Enable Simulate Asset Bundles", true)]
	private static bool GetEnableCachedInfoVal()
	{
		return !EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES ? true : false;
	}

	[MenuItem("_Core / Disable Simulate Asset Bundles")]
	private static void GetDisableCachedInfo()
	{
		CreateRedBlueGameObject();

		EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES = false;
		Debug.Log(("Disabled asset bundle simulation mode.").Colored(Colors.yellow));
	}

	[MenuItem("_Core / Disable Simulate Asset Bundles", true)]
	private static bool GetDisableCachedInfoVal()
	{
		return EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES ? true : false;
	}
}