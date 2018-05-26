using UnityEditor;
using UnityEngine;

namespace Core
{
    public class EditorStartup
    {
        [InitializeOnLoad]
        public class Startup
        {
            static Startup()
            {
                if (EditorPreferences.EDITORPREF_FIRST_TIME_USE)
                {
                    Debug.Log("First time use. Enabling asset bundle simulation mode.".Colored(Colors.Yellow));

                    EditorPreferences.EDITORPREF_SIMULATE_ASSET_BUNDLES = true;
                    EditorPreferences.EDITORPREF_FIRST_TIME_USE = false;
                }
            }
        }
    }
}