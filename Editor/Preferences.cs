using UnityEditor;
using UnityEngine;

namespace Core.Services
{
	public class Preferences
	{
		private static bool prefsLoaded = false;
		private static bool simulateAssetBundles = false;

		[PreferenceItem("Core Framework")]
		private static void CustomPreferencesGUI()
		{
			if (!prefsLoaded)
			{
				simulateAssetBundles = EditorPreferences.EditorprefSimulateAssetBundles;
				prefsLoaded = true;
			}

			EditorUITools.Header("Asset Bundles");
			simulateAssetBundles = EditorGUILayout.ToggleLeft("Simulation Mode", simulateAssetBundles);

			if (GUI.changed)
			{
				EditorPreferences.EditorprefSimulateAssetBundles = simulateAssetBundles;
			}

			EditorUITools.HorizontalLine();
		}
	}
}