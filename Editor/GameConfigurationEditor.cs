using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Core.Service
{
	[CustomEditor(typeof(GameConfiguration))]
	public class GameConfigurationEditor : Editor
	{
		private ReorderableList services;
		GameConfiguration gameConfiguration;

		public override void OnInspectorGUI()
		{
			gameConfiguration = target as GameConfiguration;

			gameConfiguration.disableLogging = EditorGUILayout.ToggleLeft("Disable Debug.Log?", gameConfiguration.disableLogging);

			GUILayout.Space(10);
			serializedObject.Update();
			services.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}
		private void OnEnable()
		{
			Color backColor = GUI.backgroundColor;
			Color contentColor = GUI.contentColor;
			float line = EditorGUIUtility.singleLineHeight;
			SerializedProperty property = serializedObject.FindProperty("services");
			services = new ReorderableList(serializedObject, property);
			services.showDefaultBackground = true;
			services.elementHeight = line + 6;

			services.drawHeaderCallback = (rect) =>
			{
				EditorGUI.LabelField(rect, "Core Framework Services");
			};

			services.drawElementCallback = (Rect rect, int index, bool active, bool focused) =>
			{
				float width = rect.width - 22;
				SerializedProperty element = services.serializedProperty.GetArrayElementAtIndex(index);

				if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y + 4, 18, line), EditorGUIUtility.IconContent("_Popup").image, GUIStyle.none))
				{
					GenericMenu menu = new GenericMenu();
					menu.AddItem(new GUIContent("Edit"), false, delegate()
					{
						Selection.activeObject = element.objectReferenceValue;
					});

					menu.AddItem(new GUIContent("Remove"), false, delegate()
					{
						if (!gameConfiguration.services[index])
						{
							gameConfiguration.services.RemoveAt(index);
							EditorUtility.SetDirty(gameConfiguration);
						}
						else if (EditorUtility.DisplayDialog("Remove " + ObjectNames.NicifyVariableName(gameConfiguration.services[index].name) + "?", "Are you sure you want to remove this service? This will delete the ScriptableObject and can't be undone.", "Yes", "No"))
						{
							AssetDatabase.DeleteAsset(AssetDatabase.GetAssetOrScenePath(element.objectReferenceValue));
							gameConfiguration.services.RemoveAt(index);
							EditorUtility.SetDirty(gameConfiguration);
						}
					});

					menu.ShowAsContext();
				}

				EditorGUI.PropertyField(new Rect(rect.x, rect.y + 2, width, line), element, GUIContent.none);
			};

			services.onAddCallback = (list) =>
			{
				Type[] allServiceTypes = GetAllServiceTypes();
				GenericMenu servicesMenu = new GenericMenu();
				foreach (var serviceType in allServiceTypes)
				{
					servicesMenu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(serviceType.Name)), false, delegate(object service)
					{
						Type styp = service as Type;
						foreach (var assy in AppDomain.CurrentDomain.GetAssemblies())
						{
							foreach (var typ in assy.GetTypes())
							{
								if (typ.Name == styp.Name + "Config" || typ.Name == styp.Name + "Configuration")
								{
									serializedObject.ApplyModifiedProperties();
									object thing = ScriptableObject.CreateInstance(typ);
									if (thing == null || (thing as ServiceConfiguration) == null) return;
									serializedObject.ApplyModifiedProperties();
									string path = AssetDatabase.GetAssetOrScenePath(serializedObject.targetObject);
									path = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar + styp.Name + ".asset";
									AssetDatabase.CreateAsset(thing as ServiceConfiguration, path);
									if (thing != null)
									{
										gameConfiguration.services.Add(thing as ServiceConfiguration);
										EditorUtility.SetDirty(gameConfiguration);
									}
									return;
								}
							}
						}

					}, serviceType);
				}
				servicesMenu.ShowAsContext();
			};
		}

		public static Type[] GetAllServiceTypes()
		{
			return (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies() from assemblyType in domainAssembly.GetTypes() where(typeof(IService).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract) select assemblyType).ToArray();
		}
	}
}