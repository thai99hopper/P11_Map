
using UnityEditor;
using UnityEngine;

public partial class EditorUIElementCreator
{
	public static void CreateLabelField(string text)
	{
		var size = StaticUtilsEditor.CalculateTextSize(text, EditorUIComponentType.label);
		var width = size.x;
		EditorGUILayout.LabelField(text, GUILayout.Width(width));
	}

	public static void CreateLabelField_center(string text, float parentWidth, float parentHeight, int fontSize)
	{
		var style = new GUIStyle(GUI.skin.label)
		{
			alignment = TextAnchor.MiddleCenter,
			fixedWidth = parentWidth,
			fixedHeight = parentHeight,
			fontSize = fontSize,
		};

		EditorGUILayout.LabelField(text, style);
	}
}