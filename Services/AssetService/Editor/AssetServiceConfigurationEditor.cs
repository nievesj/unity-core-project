using Core.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Core.Services.Assets
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
				EditorUITools.HorizontalLine();

				if (!configuration.AssetBundlesURL.Equals(string.Empty))
				{
					configuration.UseCache = EditorGUILayout.ToggleLeft("Cache Asset Bundles?", configuration.UseCache);

					EditorUITools.HorizontalLine();

					EditorGUILayout.LabelField("Manifest Cache Expiring Period in Days?");
					configuration.ManifestCachePeriod = EditorGUILayout.IntSlider(configuration.ManifestCachePeriod, 1, 90);
				}
				else
				{
					EditorGUILayout.LabelField("Invalid URL");
				}

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