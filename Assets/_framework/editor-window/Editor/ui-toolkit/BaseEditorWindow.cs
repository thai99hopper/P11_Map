using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class BaseEditorWindow : EditorWindow
{
	#region core

	[SerializeField]
	protected VisualTreeAsset mainLayout;

	protected static void OpenWindow<T>() where T : EditorWindow
	{
		var window = GetWindow<T>();
		window.titleContent = new GUIContent(typeof(T).Name);
	}
	
	public void CreateGUI()
	{
		var layoutAsset = GetDefaultLayout();
		layoutAsset.CloneTree(rootVisualElement);

		SetupControlHandler(layoutAsset);
	}

	#endregion

	#region utils

	protected void ChangeLayout(VisualTreeAsset layoutAsset)
	{
		rootVisualElement.Clear();
		layoutAsset.CloneTree(rootVisualElement);

		SetupControlHandler(layoutAsset);
	}
	
	#endregion
	
	#region virtual functions

	protected virtual VisualTreeAsset GetDefaultLayout()
	{
		return mainLayout;
	}

	protected virtual void SetupControlHandler(VisualTreeAsset layoutAsset)
	{
	}

	#endregion

	#region modify UI controls

	protected void AssignButtonClickEvent(string btnName, UnityAction callback)
	{
		AssignButtonClickEvent(rootVisualElement, btnName, callback);
	}

	protected void AssignButtonClickEvent(VisualElement root, string btnName, UnityAction callback)
	{
		var btn = root.Q<Button>(name: btnName);
		btn.RegisterCallback<ClickEvent>(_ =>
		{
			callback?.Invoke();
		});
	}

	protected void SetTextLabel(string labelName, string txt)
	{
		SetTextLabel(rootVisualElement, labelName, txt);
	}

	protected void SetTextLabel(VisualElement root, string labelName, string txt)
	{
		var label = root.Q<Label>(name: labelName);
		label.text = txt;
	}

	#endregion
}