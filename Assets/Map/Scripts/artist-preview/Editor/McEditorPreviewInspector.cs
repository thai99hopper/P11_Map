
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(McEditorPreview))]
public class McEditorPreviewInspector : Editor
{
    public class AreaInfo
    {
        public string name;
        public bool isShowed = false;
        public bool isShowMc = false;
        public Dictionary<string, GameObject> mcGo;

        public AreaInfo(string name)
        {
            this.name = name;
            this.isShowed = false;
            this.isShowMc = false;
            this.mcGo = new Dictionary<string, GameObject>();
        }
    }

    private static string _SpineMcPrefabPath = null;
    public static string SpineMcPrefabPath
    {
        get
        {
            if (_SpineMcPrefabPath != null)
                return _SpineMcPrefabPath;
            var path1 = "Assets/Map/Model/_mc/DEV";
            var path2 = "Assets/_game/Map/Model/_mc/DEV";
            _SpineMcPrefabPath = path1;
            if (Directory.Exists(path2))
                _SpineMcPrefabPath = path2;
            return _SpineMcPrefabPath;
        }
    }
    private static Dictionary<string, AreaInfo> areaInfos;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        McEditorPreview preview = (McEditorPreview)target;

        GUILayout.Space(10);
        GUILayout.Label("Preview Mc", EditorStyles.boldLabel);


        if (!IsInitialize() || !IsInitialize_Area(preview.AreaName))
            IsInitialize(preview.AreaName);
        var areaInfo = areaInfos[preview.AreaName];

        var label = areaInfo.isShowMc ? "Hide" : "Show";
        if (GUILayout.Button(label))
        {
            areaInfo.isShowMc = !areaInfo.isShowMc;
            if (areaInfo.isShowMc)
                ShowMc(preview);
            else
                HideMc(preview);
        }
    }

    public bool IsInitialize()
    {
        return areaInfos != null;
    }

    public bool IsInitialize_Area(string areaName)
    {
        if (!IsInitialize())
            return false;
        return areaInfos.ContainsKey(areaName);
    }

    public void IsInitialize(string areaName)
    {
        if (areaInfos == null)
            areaInfos = new Dictionary<string, AreaInfo>();
        if (!areaInfos.ContainsKey(areaName))
        {
            areaInfos.Add(areaName, new AreaInfo(areaName));
        }
    }

    public void ShowMc(McEditorPreview preview)
    {
        if (!IsInitialize())
            IsInitialize(preview.AreaName);

        var areaInfo = areaInfos[preview.AreaName];
        if (areaInfo.isShowed)
        {
            Debug.LogWarning($"Area {preview.AreaName} is already showed");
            return;
        }

        var progress = 0f;
        var count = 0;
        var mcPresenerList = preview.GetMcPresenterList();
        var maxCount = mcPresenerList.Count;

        Debug.Log("Show Cat Character");
        areaInfo.isShowed = true;

        EditorUtility.DisplayProgressBar("Showing Cat Characters", "Preparing...", 0f);

        foreach (var mcPresenter in mcPresenerList)
        {
            count++;
            progress = (float)count / maxCount;
            var data = AssetLoader.GetDataBuildings();
            var dataCharacter = AssetLoader.GetDataCharacterOnMap();
            var dataBuilding = data.GetBuildingData(mcPresenter.buildingId); 
            var prefabPath = dataBuilding.GetModelLoadPath(mcPresenter.orderInBuilding).modelPath;
            var name = mcPresenter.gameObject.name;
            var position = mcPresenter.transform.position;

            EditorUtility.DisplayProgressBar("Showing Cat Characters", $"Loading {name}", progress);
            
            var prefab = AssetLoader.LoadPrefab(SpineMcPrefabPath, prefabPath);
            if (prefab != null)
            {
                var mcGo = Instantiate(prefab, position, Quaternion.identity, mcPresenter.transform).GetComponent<McOnMapController>();
                mcGo.transform.localPosition = Vector3.zero;
                areaInfo.mcGo[name] = mcGo.gameObject;
                mcGo.SetupBuildingId(mcPresenter.buildingId, prefab.name);
                
                var lPos = dataCharacter.GetCharacterOnMapData(mcPresenter.buildingId, prefab.name); 
                mcGo.SetupPosition_Editor(lPos);
            }
        }
        EditorUtility.ClearProgressBar();
        Debug.Log("Show Cat Character Success!");
    }

    public void HideMc(McEditorPreview preview)
    {
        if (!IsInitialize())
            IsInitialize(preview.AreaName);
        var spineMc_area = areaInfos[preview.AreaName];

        if (spineMc_area.isShowed && spineMc_area.mcGo != null)
        {
            foreach (var building in spineMc_area.mcGo)
            {
                if (building.Value != null)
                    DestroyImmediate(building.Value);
            }
            spineMc_area.mcGo.Clear();
        }
        spineMc_area.isShowed = false;
    }
}