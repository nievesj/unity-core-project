using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Core.Services.Levels
{
	[CustomEditor(typeof(LevelLoaderServiceConfiguration))]
	public class LevelLoaderServiceConfigurationEditor : Editor
	{
		LevelLoaderServiceConfiguration configuration;
		private ReorderableList levels;

		private void OnEnable()
		{
			Color backColor = GUI.backgroundColor;
			Color contentColor = GUI.contentColor;
			float line = EditorGUIUtility.singleLineHeight;

			SerializedProperty property = serializedObject.FindProperty("levels");
			levels = new ReorderableList(serializedObject, property);
			levels.showDefaultBackground = true;
			levels.elementHeight = line + 6;

			levels.drawHeaderCallback = (rect)=>
			{
				EditorGUI.LabelField(rect, "Game Levels");
			};

			levels.drawElementCallback = (Rect rect, int index, bool active, bool focused)=>
			{
				float width = rect.width - 22;
				SerializedProperty element = levels.serializedProperty.GetArrayElementAtIndex(index);

				if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y + 4, 18, line), EditorGUIUtility.IconContent("_Popup").image, GUIStyle.none))
				{
					GenericMenu menu = new GenericMenu();
					menu.AddItem(new GUIContent("Edit"), false, delegate()
					{
						Selection.activeObject = element.objectReferenceValue;
					});

					menu.AddItem(new GUIContent("Remove"), false, delegate()
					{
						if (!string.IsNullOrEmpty(configuration.levels[index]))
						{
							configuration.levels.RemoveAt(index);
							EditorUtility.SetDirty(configuration);
						}
						else if (EditorUtility.DisplayDialog("Remove " + ObjectNames.NicifyVariableName(configuration.levels[index])+ "?", "Are you sure you want to remove this level?", "Yes", "No"))
						{
							configuration.levels.RemoveAt(index);
							EditorUtility.SetDirty(configuration);
						}
					});

					menu.ShowAsContext();
				}

				EditorGUI.PropertyField(new Rect(rect.x, rect.y + 2, width, line), element, GUIContent.none);
			};

			levels.onAddCallback = (list)=>
			{
				List<Level> availableLevels = GetAllAvailableLevels();
				GenericMenu servicesMenu = new GenericMenu();

				foreach (var level in availableLevels)
				{
					servicesMenu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(level.gameObject.name)), false, delegate(object selectedLevel)
					{
						Level lvl = selectedLevel as Level;

						serializedObject.ApplyModifiedProperties();
						configuration.levels.Add(lvl.name);

						return;

					}, level);
				}
				servicesMenu.ShowAsContext();
			};
		}

		public override void OnInspectorGUI()
		{
			configuration = target as LevelLoaderServiceConfiguration;

			GUILayout.Space(10);
			serializedObject.Update();
			levels.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}

		public static List<Level> GetAllAvailableLevels()
		{
			return FindAssetsByType<Level>();
		}

		public static List<T> FindAssetsByType<T>()where T : UnityEngine.Object
		{
			List<T> assets = new List<T>();
			string[] guids = AssetDatabase.FindAssets(string.Format("t:Prefab", typeof(UnityEngine.Object)));
			for (int i = 0; i < guids.Length; i++)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
				T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

				if (asset != null)
					assets.Add(asset);
			}

			return assets;
		}
	}
}