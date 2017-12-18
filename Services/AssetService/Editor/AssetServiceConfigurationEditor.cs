using System.Collections;
using System.Collections.Generic;
using Core.Service;
using UnityEditor;
using UnityEngine;

namespace Core.Assets
{
	[CustomEditor(typeof(AssetServiceConfiguration))]
	public class AssetServiceConfigurationEditor : Editor
	{
		AssetServiceConfiguration configuration;
		bool cachedUseStreamingAssets = false;

		private void OnEnable()
		{
			Color backColor = GUI.backgroundColor;
			Color contentColor = GUI.contentColor;
			float line = EditorGUIUtility.singleLineHeight;
		}

		public override void OnInspectorGUI()
		{
			configuration = target as AssetServiceConfiguration;

			GUILayout.Label("Asset Service Configuration Options ", EditorUITools.HeaderStyle);
			cachedUseStreamingAssets = configuration.UseStreamingAssets;
			configuration.UseStreamingAssets = EditorGUILayout.ToggleLeft("Use Streaming Assets?", configuration.UseStreamingAssets);

			if (!configuration.UseStreamingAssets)
			{
				EditorUITools.HorizontalLine();

				configuration.AssetBundlesURL = EditorGUILayout.TextField("Asset Bundles URL", configuration.AssetBundlesURL);
				configuration.UseCache = EditorGUILayout.ToggleLeft("Cache Asset Bundles?", configuration.UseCache);
			}

			if (cachedUseStreamingAssets != configuration.UseStreamingAssets)
			{
				cachedUseStreamingAssets = configuration.UseStreamingAssets;

				EditorUtility.SetDirty(target);
				AssetDatabase.SaveAssets();
			}
		}
	}
}