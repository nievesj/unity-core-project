using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Core.Services
{
	public static class EditorUITools
	{
		//Tidy header style.
		public static GUIStyle HeaderStyle
		{
			get
			{
				var headerStyle = new GUIStyle((GUIStyle)
					"MeTransOffLeft");

				headerStyle.fontSize = 14;
				headerStyle.fontStyle = FontStyle.Bold;
				headerStyle.fixedHeight = 20;

				return headerStyle;
			}
		}

		public static void Header(string label)
		{
			GUILayout.Label(label, EditorUITools.HeaderStyle);
		}

		/// <summary>
		/// Creates a horizontal line
		/// </summary>
		public static void HorizontalLine()
		{
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUILayout.EndVertical();
		}

		/// <summary>
		/// Creates a new inspector label
		/// </summary>
		/// <param name="message"></param>
		/// <param name="style">  </param>
		public static void LabelUIElement(string message, GUIStyle style)
		{
			GUILayout.BeginVertical();
			EditorGUILayout.LabelField(message, style);
			GUILayout.EndVertical();
		}

		/// <summary>
		/// Returns a label style depending on the messageType
		/// </summary>
		/// <param name="messageType"></param>
		/// <returns></returns>
		public static GUIStyle LabelStyle(LabelMessageType messageType)
		{
			GUIStyle s = new GUIStyle(EditorStyles.textField);

			switch (messageType)
			{
				case LabelMessageType.GREEN:
					s.normal.textColor = Color.green;
					break;

				case LabelMessageType.RED:
					s.normal.textColor = Color.red;
					break;

				case LabelMessageType.WHITE:
					s.normal.textColor = Color.white;
					break;

				case LabelMessageType.WHITEBOLD:
					s.normal.textColor = Color.white;
					s.fontStyle = FontStyle.Bold;
					break;
			}

			s.wordWrap = true;

			return s;
		}

		/// <summary>
		/// Gathers and returns all sorting layers created within the editor. This method relies on
		/// UnityEditorInternal which could break in future Unity versions.
		/// </summary>
		/// <returns></returns>
		public static string[] GetSortingLayerNames()
		{
			System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
			var sortingLayers = (string[])sortingLayersProperty.GetValue(null, new object[0]);
			return sortingLayers;
		}

		public enum LabelMessageType
		{
			GREEN,
			RED,
			WHITE,
			WHITEBOLD
		}
	}
}