using UnityEditor;
using UnityEngine;

namespace Core.Services
{
	public class Preferences : MonoBehaviour
	{
		// Have we loaded the prefs yet
		private static bool prefsLoaded = false;
		// The Preferences
		private static bool boolPreference = false;

		[PreferenceItem("Core Framework")]
		private static void CustomPreferencesGUI()
		{
			if (!prefsLoaded)
			{
				boolPreference = EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES;
				prefsLoaded = true;
			}

			EditorUITools.Header("Asset Bundles");
			boolPreference = EditorGUILayout.ToggleLeft("Simulation Mode", boolPreference);

			if (GUI.changed)
			{
				EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES = boolPreference;
			}

			EditorUITools.HorizontalLine();
		}
	}
}