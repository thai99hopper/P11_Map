
using System.Collections.Generic;

public class EditorUIElement_tabWindow: EditorUIElement_tab
{
	public abstract class TabItemWindow : TabItem
	{
		public EditorWindowStateMachine FSM;
	}

	public EditorUIElement_tabWindow(List<TabItem> listTabItems, EditorWindowStateMachine FSM)
		: base(listTabItems)
	{
		foreach (var i in listTabItems)
		{
			((TabItemWindow)i).FSM = FSM;
		}
	}
}