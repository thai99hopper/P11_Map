using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(CameraController))]
public class CameraControllerCustomInspector: BaseCustomInspector
{
	protected override void SetupControlHandler()
	{
		base.SetupControlHandler();

		SetupCameraConfig("pan-config", "enable-pan-config");
		SetupCameraConfig("zoom-config", "enable-zoom-config");
	}

	private void SetupCameraConfig(string parentName, string toggleName)
	{
		AssignToggleSwitchEvent(toggleName, val =>
		{
			SetEnableConfig(parentName, toggleName, val);
		});

		var toggle = FindControlByName<Toggle>(toggleName);
		SetEnableConfig(parentName, toggleName, toggle.value);
	}

	private void SetEnableConfig(string parentName, string toggleName, bool enable)
	{
		var parent = FindControlByName(parentName);
		var children = parent.Children();
		foreach (var child in children)
		{
			if (!child.name.Equals(toggleName))
			{
				child.visible = enable;
			}
		}
	}
}