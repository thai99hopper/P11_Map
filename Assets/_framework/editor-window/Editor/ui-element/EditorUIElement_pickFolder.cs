
using UnityEditor;
using UnityEngine;

public class EditorUIElement_pickFolder
{
	private string lable;
	private string persistentKey;

	public string PickedPath { get; set; } = "";

	public EditorUIElement_pickFolder(string lable)
		:this(lable,null)
	{
	}

	public EditorUIElement_pickFolder(string lable, string persistentKey)
	{
		this.lable = lable;
		this.persistentKey = persistentKey;

		if (!string.IsNullOrEmpty(persistentKey))
		{
			PickedPath = PlayerPrefs.GetString(persistentKey, "");
		}
	}

	public void Draw()
	{
		EditorGUILayout.BeginHorizontal();

		EditorUIElementCreator.CreateLabelField(lable);
		if (GUILayout.Button(PickedPath, GUI.skin.textField))
		{
			var newPath = OpenDialog();
			if (!string.IsNullOrEmpty(newPath))
			{
				PickedPath = newPath;
				if (!string.IsNullOrEmpty(persistentKey))
				{
					PlayerPrefs.SetString(persistentKey, newPath);
				}
			}
		}

		EditorGUILayout.EndHorizontal();
	}

	protected virtual string OpenDialog()
	{
		return StaticUtilsEditor.DisplayChooseFolderDialog("choose folder");
	}
}