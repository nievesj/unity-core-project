using UnityEditor;
using UnityEngine;

namespace Core.Services.Assets
{
    [CustomEditor(typeof(AssetServiceConfiguration))]
    public class AssetServiceConfigurationEditor : Editor
    {
        private AssetServiceConfiguration _configuration;

        private void OnEnable()
        {
            var backColor = GUI.backgroundColor;
            var contentColor = GUI.contentColor;
            var line = EditorGUIUtility.singleLineHeight;
        }

        public override void OnInspectorGUI()
        {
            _configuration = target as AssetServiceConfiguration;

            GUILayout.Label("Asset Service Configuration Options ", EditorUITools.HeaderStyle);
            _configuration.UseStreamingAssets = EditorGUILayout.ToggleLeft("Use Streaming Assets?", _configuration.UseStreamingAssets);

            if (!_configuration.UseStreamingAssets)
            {
                EditorUITools.HorizontalLine();

                _configuration.AssetBundlesURL = EditorGUILayout.TextField("Asset Bundles URL", _configuration.AssetBundlesURL);
                EditorUITools.HorizontalLine();

                if (!_configuration.AssetBundlesURL.Equals(string.Empty))
                {
                    _configuration.UseCache = EditorGUILayout.ToggleLeft("Cache Asset Bundles?", _configuration.UseCache);

                    if (_configuration.UseCache)
                        _configuration.UseUnityCloudBuildManifestVersion = true;
                    else
                        _configuration.UseUnityCloudBuildManifestVersion = false;
                }
                else
                {
                    EditorGUILayout.LabelField("Invalid URL");
                }
            }
            else
            {
                _configuration.UseCache = true;
            }

            EditorUtility.SetDirty(target);
            // AssetDatabase.SaveAssets();
        }
    }
}