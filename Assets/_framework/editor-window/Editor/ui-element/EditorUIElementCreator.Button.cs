
using UnityEngine;

public partial class EditorUIElementCreator
{
	public static bool CreateButton(string text)
	{
		var size = StaticUtilsEditor.CalculateTextSize(text, EditorUIComponentType.button);
		var width = size.x;
		return GUILayout.Button(text, GUILayout.Width(width));
	}
}