
using System.Collections.Generic;

public class EditorUIElement_pickFile : EditorUIElement_pickFolder
{
	private List<string> lExtension;

	public EditorUIElement_pickFile(List<string> lExtension, string lable)
		: this(lExtension, lable, null)
	{
	}

	public EditorUIElement_pickFile(List<string> lExtension, string lable, string persistentKey)
		: base(lable, persistentKey)
	{
		this.lExtension = lExtension;
	}

	protected override string OpenDialog()
	{
		return StaticUtilsEditor.DisplayChooseFileDialog("choose file", lExtension);
	}
}