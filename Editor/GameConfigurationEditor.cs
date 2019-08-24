using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Core.Services
{
    [CustomEditor(typeof(GameConfiguration))]
    public class GameConfigurationEditor : Editor
    {
        private ReorderableList _services;
        private GameConfiguration _gameConfiguration;

        public override void OnInspectorGUI()
        {
            _gameConfiguration = target as GameConfiguration;
            _gameConfiguration.DisableLogging = EditorGUILayout.ToggleLeft("Disable Debug.Log?", _gameConfiguration.DisableLogging);

            GUILayout.Space(10);
            serializedObject.Update();
            _services.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            Color backColor = GUI.backgroundColor;
            Color contentColor = GUI.contentColor;
            float line = EditorGUIUtility.singleLineHeight;
            SerializedProperty property = serializedObject.FindProperty("services");
            _services = new ReorderableList(serializedObject, property);
            _services.showDefaultBackground = true;
            _services.elementHeight = line + 6;

            _services.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "Core Framework Services"); };

            _services.drawElementCallback = (Rect rect, int index, bool active, bool focused) =>
            {
                float width = rect.width - 22;
                SerializedProperty element = _services.serializedProperty.GetArrayElementAtIndex(index);

                if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y + 4, 18, line), EditorGUIUtility.IconContent("_Popup").image, GUIStyle.none))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Edit"), false, delegate() { Selection.activeObject = element.objectReferenceValue; });

                    menu.AddItem(new GUIContent("Remove"), false, delegate()
                    {
                        if (!_gameConfiguration.services[index])
                        {
                            _gameConfiguration.services.RemoveAt(index);
                            EditorUtility.SetDirty(_gameConfiguration);
                        }
                        else if (EditorUtility.DisplayDialog("Remove " + ObjectNames.NicifyVariableName(_gameConfiguration.services[index].name) + "?", "Are you sure you want to remove this service? This will delete the ScriptableObject and can't be undone.", "Yes", "No"))
                        {
                            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetOrScenePath(element.objectReferenceValue));
                            _gameConfiguration.services.RemoveAt(index);
                            EditorUtility.SetDirty(_gameConfiguration);
                        }
                    });

                    menu.ShowAsContext();
                }

                EditorGUI.PropertyField(new Rect(rect.x, rect.y + 2, width, line), element, GUIContent.none);
            };

            _services.onAddCallback = (list) =>
            {
                Type[] allServiceTypes = GetAllServiceTypes();
                GenericMenu servicesMenu = new GenericMenu();
                foreach (var serviceType in allServiceTypes)
                    servicesMenu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(serviceType.Name)), false, delegate(object service)
                    {
                        Type styp = service as Type;
                        foreach (var assy in AppDomain.CurrentDomain.GetAssemblies())
                        foreach (var typ in assy.GetTypes())
                            if (typ.Name == styp.Name + "Config" || typ.Name == styp.Name + "Configuration")
                            {
                                serializedObject.ApplyModifiedProperties();
                                object so = ScriptableObject.CreateInstance(typ);
                                if (so == null || so as ServiceConfiguration == null) return;
                                serializedObject.ApplyModifiedProperties();
                                string path = AssetDatabase.GetAssetOrScenePath(serializedObject.targetObject);
                                path = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar + styp.Name + ".asset";
                                AssetDatabase.CreateAsset(so as ServiceConfiguration, path);
                                if (so != null)
                                {
                                    _gameConfiguration.services.Add(so as ServiceConfiguration);
                                    EditorUtility.SetDirty(_gameConfiguration);
                                }

                                return;
                            }
                    }, serviceType);
                servicesMenu.ShowAsContext();
            };
        }

        public static Type[] GetAllServiceTypes()
        {
            return (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies() from assemblyType in domainAssembly.GetTypes() where typeof(Service).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract select assemblyType).ToArray();
        }
    }
}