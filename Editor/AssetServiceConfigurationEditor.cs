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
			configuration.UseStreamingAssets = EditorGUILayout.ToggleLeft("Use Streaming Assets?", configuration.UseStreamingAssets);

			if (!configuration.UseStreamingAssets)
			{
				EditorUITools.HorizontalLine();

				configuration.AssetBundlesURL = EditorGUILayout.TextField("Asset Bundles URL", configuration.AssetBundlesURL);
				EditorUITools.HorizontalLine();

				if (!configuration.AssetBundlesURL.Equals(string.Empty))
				{

					configuration.UseCache = EditorGUILayout.ToggleLeft("Cache Asset Bundles?", configuration.UseCache);
					if (configuration.CacheBundleManifestsLocally == false && configuration.UseUnityCloudBuildManifestVersion == false)
						configuration.CacheBundleManifestsLocally = true;

					if (configuration.UseCache)
					{

						EditorUITools.HorizontalLine();

						configuration.CacheBundleManifestsLocally = EditorGUILayout.ToggleLeft("Cache bundles by caching .manifest files locally?", configuration.CacheBundleManifestsLocally);
						if (configuration.CacheBundleManifestsLocally)
						{

							configuration.UseUnityCloudBuildManifestVersion = false;

							EditorGUILayout.LabelField("Manifest Cache Expiring Period in Days?");
							configuration.ManifestCachePeriod = EditorGUILayout.IntSlider(configuration.ManifestCachePeriod, 1, 90);

							EditorUITools.HorizontalLine();
						}
						else
						{

							configuration.UseUnityCloudBuildManifestVersion = true;
						}

						configuration.UseUnityCloudBuildManifestVersion = EditorGUILayout.ToggleLeft("Cache bundles by using Unity Cloud Build Manifest?", configuration.UseUnityCloudBuildManifestVersion);
						if (configuration.UseUnityCloudBuildManifestVersion)
						{
							configuration.CacheBundleManifestsLocally = false;
						}
						else
						{
							configuration.CacheBundleManifestsLocally = true;
						}
					}
					else
					{
						configuration.UseStreamingAssets = true;
					}
				}
				else
				{
					EditorGUILayout.LabelField("Invalid URL");
				}
			}
			else
			{
				configuration.UseCache = true;
			}

			EditorUtility.SetDirty(target);
			// AssetDatabase.SaveAssets();
		}
	}
}