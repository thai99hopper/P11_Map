
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemController : SingletonMonoBehaviour<EventSystemController>
{
	public EventSystem eventSystem;

	public bool ClickedOnAnyUI(Vector3 clickedPos)
	{
		var data = new PointerEventData(eventSystem)
		{
			position = clickedPos,
		};
		var result = new List<RaycastResult>();
		eventSystem.RaycastAll(data, result);
		return result.Count > 0;
	}
}