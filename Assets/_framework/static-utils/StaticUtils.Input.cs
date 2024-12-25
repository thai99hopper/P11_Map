using UnityEngine;

public static partial class StaticUtils
{
	#region mouse

	public static float GetMouseScrollDelta()
	{
		return Input.mouseScrollDelta.y;
	}

	#endregion

	#region touch

	public static bool IsBeginTouchScreen(MouseButtonType type)
	{
		return Input.GetMouseButtonDown((int)type);
	}

	public static bool IsTouchingScreen(MouseButtonType type)
	{
		return Input.GetMouseButton((int)type);
	}

	public static bool IsEndTouchScreen(MouseButtonType type)
	{
		return Input.GetMouseButtonUp((int)type);
	}

	public static bool IsBeginTouchScreen()
	{
		return IsBeginTouchScreen(MouseButtonType.LeftMouse);
	}

	public static bool IsTouchingScreen()
	{
		return IsTouchingScreen(MouseButtonType.LeftMouse);
	}

	public static bool IsEndTouchScreen()
	{
		return IsEndTouchScreen(MouseButtonType.LeftMouse);
	}

	public static Vector3 GetTouchPosition()
	{
		return Input.mousePosition;
	}

	#endregion

	#region key

	public static bool IsPressBackKey()
	{
		return Input.GetKeyDown(KeyCode.Escape);
	}

	public static bool IsPressCombinationCtrl(KeyCode keyCode)
	{
		return (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
			Input.GetKeyDown(keyCode);
	}

	public static bool IsPressCombinationAlt(KeyCode keyCode)
	{
		return (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) &&
			Input.GetKeyDown(keyCode);
	}

	public static bool IsPressCombinationShift(KeyCode keyCode)
	{
		return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
			Input.GetKeyDown(keyCode);
	}

	#endregion
}