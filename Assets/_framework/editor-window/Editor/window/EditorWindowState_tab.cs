
using System.Collections.Generic;

public class EditorWindowState_tab : EditorWindowState
{
	private EditorUIElement_tab tabView;
	private List<EditorUIElement_tab.TabItem> tabItems;

	public EditorWindowState_tab(List<EditorUIElement_tab.TabItem> tabItems)
	{
		this.tabItems = tabItems;
	}

	public override void OnBeginState()
	{
		base.OnBeginState();

		//init tabView here in order for FSM valid
		tabView = new EditorUIElement_tabWindow(tabItems, FSM);
	}

	public override void OnDraw()
	{
		tabView.Draw();
	}

	public override void OnWindowClosed()
	{
		base.OnWindowClosed();

		foreach (var i in tabItems)
		{
			i.OnWindowClosed();
		}
	}
}