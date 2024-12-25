
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class BaseEditorWindow : EditorWindow
{
    [SerializeField]
    protected VisualTreeAsset mainLayout;

    protected static void OpenWindow<T>() where T : EditorWindow
    {
        var window = GetWindow<T>();
        window.titleContent = new GUIContent(typeof(T).Name);
    }

    protected virtual VisualTreeAsset GetDefaultLayout()
    {
        return mainLayout;
    }

    protected virtual void SetupControlHandler()
    {
    }

    public void CreateGUI()
    {
        var layoutAsset = GetDefaultLayout();
        layoutAsset.CloneTree(rootVisualElement);

        SetupControlHandler();
    }

    protected void ChangeLayout(VisualTreeAsset layoutAsset)
    {
        rootVisualElement.Clear();
        layoutAsset.CloneTree(rootVisualElement);
    }

    private T GetElement<T>(string name) where T : VisualElement
    {
        T result = null;
        var l = rootVisualElement.Query<T>();
        l.ForEach(i =>
        {
            if (i.name.Equals(name))
            {
                result = i;
            }
        });
        if (result != null)
        {
            return result;
        }
        else
        {
            throw new Exception($"no {typeof(T).Name} with name {name}");
        }
    }

    #region modify UI controls

    protected void AssignButtonClickEvent(string btnName, UnityAction callback)
    {
        var btn = GetElement<Button>(btnName);
        btn.RegisterCallback<ClickEvent>(_ =>
        {
            callback?.Invoke();
        });
    }

    protected void SetTextLabel(string labelName, string txt)
    {
        var label = GetElement<Label>(labelName);
        label.text = txt;
    }

    #endregion
}