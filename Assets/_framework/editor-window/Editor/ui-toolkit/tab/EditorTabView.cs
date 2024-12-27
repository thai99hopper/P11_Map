
using System.Collections.Generic;
using UnityEngine.UIElements;

public static class EditorTabView
{
    private static VisualElement root;
    
    public static void Init(VisualElement root)
    {
        EditorTabView.root = root;
        var list = GetTabBtn();
        var firstItem = true;
        foreach (var i in list)
        {
            if (firstItem)
            {
                SelectTab(i);
            }
            else
            {
                UnselectTab(i);
            }
            firstItem=false;
            
            i.RegisterCallback<ClickEvent>(OnClickTab);
        }
    }

    private static List<Label> GetTabBtn()
    {
        var list = new List<Label>();
        var tList = root.Query<Label>(className: "tab_btn");
        tList.ForEach(i =>
        {
            list.Add(i);
        });
        return list;
    }
    
    private static void OnClickTab(ClickEvent evt)
    {
        var label = evt.currentTarget as Label;
        var list = GetTabBtn();
        foreach (var i in list)
        {
            if (i == label)
            {
                SelectTab(i);
            }
            else
            {
                UnselectTab(i);
            }
        }
    }

    private static void SelectTab(Label tab)
    {
        tab.AddToClassList("selected_tab_btn");
        var content = GetTabContent(tab);
        content.RemoveFromClassList("unselected_tab_content");
    }

    private static void UnselectTab(Label tab)
    {
        tab.RemoveFromClassList("selected_tab_btn");
        var content = GetTabContent(tab);
        content.AddToClassList("unselected_tab_content");
    }

    private static VisualElement GetTabContent(Label tabBtn)
    {
        var name = tabBtn.name;
        name = name.Replace("_btn_", "_content_");
        return root.Q(name);
    }
}
