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
                if (EditorPreferences.EditorprefFirstTimeUse)
                {
                    Debug.Log("First time use. Enabling asset bundle simulation mode.".Colored(Colors.Yellow));

                    EditorPreferences.EditorprefSimulateAssetBundles = true;
                    EditorPreferences.EditorprefFirstTimeUse = false;
                }
            }
        }
    }
}