using System;
using System.Collections.Generic;
using UnityEditor;

public partial class EditorUIElementCreator
{
	public static string CreatePopup_string(string label, string val, string[] options, out bool hasChanged)
	{
		EditorGUILayout.BeginHorizontal();

		if (!string.IsNullOrEmpty(label))
		{
			CreateLabelField(label);
		}
		var idx = options.IndexOf(val);
		idx = EditorGUILayout.Popup(idx, options);

		EditorGUILayout.EndHorizontal();

		var newVal = options[idx];
		hasChanged = !val.Equals(newVal);
		return newVal;
	}

	public static T CreatePopup_enum<T>(string label, T val, List<T> options, out bool hasChanged) where T : struct, IConvertible
	{
		var optionsAsStr = new string[options.Count];
		for (var i = 0; i < options.Count; i++)
		{
			optionsAsStr[i] = options[i].ToString();
		}

		var resultAsStr = CreatePopup_string(label, val.ToString(), optionsAsStr, out hasChanged);
		return StaticUtils.StringToEnum<T>(resultAsStr);
	}

	public static T CreatePopup_enum<T>(string label, T numVal, T val, out bool hasChanged) where T : struct, IConvertible
	{
		var numValAsInt = Convert.ToInt32(numVal);
		var options = new List<T>();
		for (var i = 0; i < numValAsInt; i++)
		{
			var enumVal = StaticUtils.IntToEnum<T>(i);
			options.Add(enumVal);
		}

		return CreatePopup_enum(label, val, options, out hasChanged);
	}
}