
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorUIElement_tabInspector<T> : EditorUIElement_tab where T : Object
{
	public abstract class TabItem<T1> : TabItem where T1 : Object
	{
		public SerializedObject serializedObject;
		public T1 targetObject;
		public Editor inspector;
	}

	public EditorUIElement_tabInspector(List<TabItem> listTabItems, SerializedObject serializedObject, Editor inspector)
		:base(listTabItems)
	{
		foreach (var i in listTabItems)
		{
			var item = (TabItem<T>)i;
			item.serializedObject = serializedObject;
			item.targetObject = (T)serializedObject.targetObject;
			item.inspector = inspector;
		}
	}
}