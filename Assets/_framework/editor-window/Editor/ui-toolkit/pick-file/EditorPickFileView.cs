
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EditorPickFileView
{
    private List<string> lExtension;
    private string persistentKey;
    private Label pathField;
    public string PickedPath { get; set; } = "";

    public EditorPickFileView(VisualElement root, List<string> lExtension) : this(root, lExtension, null)
    {
    }

    public EditorPickFileView(VisualElement root, List<string> lExtension, string persistentKey)
    {
        this.persistentKey = persistentKey;
        this.lExtension = lExtension;

        pathField = root.Q<Label>(className: "pick_file_path");
        pathField.RegisterCallback<ClickEvent>(OnClickPathField);

        if (!string.IsNullOrEmpty(persistentKey))
        {
            PickedPath = PlayerPrefs.GetString(persistentKey, "");
            pathField.text = PickedPath;
        }
    }

    private void OnClickPathField(ClickEvent evt)
    {
        var path = StaticUtilsEditor.DisplayChooseFileDialog("choose file", lExtension);
        if (!string.IsNullOrEmpty(path))
        {
            PickedPath = path;
            pathField.text = PickedPath;
            if (!string.IsNullOrEmpty(persistentKey))
            {
                PlayerPrefs.SetString(persistentKey, PickedPath);
            }
        }
    }
}
