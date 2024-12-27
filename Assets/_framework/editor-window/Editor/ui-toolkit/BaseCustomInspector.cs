
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.Events;

public class BaseCustomInspector: Editor
{
	[SerializeField]
	protected VisualTreeAsset layoutAsset;

	private VisualElement currentLayout;

	protected virtual void SetupControlHandler()
	{
	}

	public override VisualElement CreateInspectorGUI()
	{
		currentLayout = new VisualElement();
		layoutAsset.CloneTree(currentLayout);

		SetupControlHandler();

		return currentLayout;
	}

	protected void AssignButtonClickEvent(string btnName, UnityAction callback)
	{
		var btns = currentLayout.Query<Button>();
		btns.ForEach(btn =>
		{
			if (btn.name.Equals(btnName))
			{
				btn.RegisterCallback<ClickEvent>(_ =>
				{
					callback?.Invoke();
				});
			}
		});
	}

	protected void AssignToggleSwitchEvent(string toggleName, UnityAction<bool> callback)
	{
		var toggles = currentLayout.Query<Toggle>();
		toggles.ForEach(toggle =>
		{
			if (toggle.name.Equals(toggleName))
			{
				toggle.RegisterValueChangedCallback(val =>
				{
					callback?.Invoke(val.newValue);
				});
			}
		});
	}

	protected VisualElement FindControlByName(string name)
	{
		return currentLayout.Query(name).First();
	}

	protected T FindControlByName<T>(string name) where T:VisualElement
	{
		return currentLayout.Query<T>(name).First();
	}
}