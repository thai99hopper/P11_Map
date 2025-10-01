
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingEditorPreview))]
public class BuildingEditorPreviewInspector : Editor
{
    public class AreaInfo
    {
        public string name;
        public bool isShowed = false;
        public bool isShowBuilding = false;
        public Dictionary<string, GameObject> buildings;

        public AreaInfo(string name) 
        {
            this.name = name;
            this.isShowed = false;
            this.isShowBuilding = false;
            this.buildings = new Dictionary<string, GameObject>();
        }
    }

    private static Dictionary<string, AreaInfo> spineBuildings;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BuildingEditorPreview preview = (BuildingEditorPreview)target;

        GUILayout.Space(10);
        GUILayout.Label("Preview Building", EditorStyles.boldLabel);


        if (!IsInitialize() || !IsInitialize_Area(preview.AreaName))
            IsInitialize(preview.AreaName);
        var areaInfo = spineBuildings[preview.AreaName];

        var label = areaInfo.isShowBuilding ? "Hide" : "Show";
        if (GUILayout.Button(label))
        {
            areaInfo.isShowBuilding = !areaInfo.isShowBuilding;
            if (areaInfo.isShowBuilding)
                ShowBuilding(preview);
            else
                HideBuilding(preview);
        }

        //if (GUILayout.Button("Idle"))
        //    preview.PlayAnimation("idle");

        //if (GUILayout.Button("Run"))
        //    preview.PlayAnimation("run");

        //if (GUILayout.Button("Full"))
        //    preview.PlayAnimation("full");
    }

    public bool IsInitialize()
    {
        return spineBuildings != null;
    }

    public bool IsInitialize_Area(string areaName)
    {
        if (!IsInitialize())
            return false;
        return spineBuildings.ContainsKey(areaName);
    }

    public void IsInitialize(string areaName)
    {
        if (spineBuildings == null)
            spineBuildings = new Dictionary<string, AreaInfo>();
        if (!spineBuildings.ContainsKey(areaName))
        {
            spineBuildings.Add(areaName, new AreaInfo(areaName));
        }
    }

    public void ShowBuilding(BuildingEditorPreview preview)
    {
        if (!IsInitialize())
            IsInitialize(preview.AreaName);

        var spineBuilding_area = spineBuildings[preview.AreaName];
        if (spineBuilding_area.isShowed)
        {
            Debug.LogWarning($"Area {preview.AreaName} is already showed");
            return;
        }

        var progress = 0f;
        var count = 0;
        var buildingList = preview.GetBuldingList();
        var maxCount = buildingList.Count;

        Debug.Log("Show Building");
        spineBuilding_area.isShowed = true;

        EditorUtility.DisplayProgressBar("Showing Buildings", "Preparing...", 0f);

        foreach (var building in buildingList)
        {
            count++;
            progress = (float)count / maxCount;
            var name = building.gameObject.name;
            EditorUtility.DisplayProgressBar("Showing Buildings", $"Loading {name}", progress);
            var prefab = AssetLoader.LoadPrefab("Assets/Map/prefab/spine-buildings", name);
            if (prefab != null)
            {
                var spineObj = Instantiate(prefab, building.transform);
                spineBuilding_area.buildings[name] = spineObj;
            }
        }
        EditorUtility.ClearProgressBar();
        Debug.Log("Show Building Success!");
    }

    public void HideBuilding(BuildingEditorPreview preview)
    {
        if (!IsInitialize())
            IsInitialize(preview.AreaName);
        var spineBuilding_area = spineBuildings[preview.AreaName];

        if (spineBuilding_area.isShowed && spineBuilding_area .buildings != null)
        {
            foreach (var building in spineBuilding_area.buildings)
            {
                if (building.Value != null)
                    DestroyImmediate(building.Value);
            }
            spineBuilding_area.buildings.Clear();
        }
        spineBuilding_area.isShowed = false;
    }
}