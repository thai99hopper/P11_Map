using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ToolUpdatePartPosOnMapEditor : EditorWindow
{
    private ScriptableDataPartBuildings targetScriptableObject;
    private GameObject rootParentObject;
    private Vector2 scrollPosition;
    
    // Scan settings
    private bool autoDetectBuildingId = true;
    private string manualBuildingId = "B01";
    
    // Mapping Building ID field
    private string mappingBuildingId = "";
    private bool useCustomMapping = false;
    
    private Dictionary<string, List<ScriptableDataPartBuildings.PosData>> scannedDataByBuilding = new();
    private bool hasScannedData = false;
    
    [MenuItem("Map/ScriptableObject/Update Building Part Position On Map")]
    public static void ShowWindow()
    {
        var window = GetWindow<ToolUpdatePartPosOnMapEditor>("Part Position Tool");
        window.minSize = new Vector2(500, 500);
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        GUILayout.Label("🏗️ Part Building Position Tool v3.0", EditorStyles.boldLabel);
        GUILayout.Label("Tool quản lý vị trí parts - 3-Level Hierarchy (Building → Part → Detail)", EditorStyles.miniLabel);
        
        EditorGUILayout.Space();

        DrawScanSection();
        EditorGUILayout.Space(10);
        
        DrawScriptableObjectSection();
        EditorGUILayout.Space(10);
        
        DrawDataPreview();
        EditorGUILayout.Space(10);
        
        DrawActionButtons();
        
        EditorGUILayout.EndScrollView();
    }

    private void DrawScanSection()
    {
        GUILayout.Label("🔍 BƯỚC 1: Scan Objects từ Scene", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("🎯 Root Object:", GUILayout.Width(100));
        rootParentObject = (GameObject)EditorGUILayout.ObjectField(rootParentObject, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();
        
        if (rootParentObject != null)
        {
            EditorGUILayout.HelpBox($"✅ Đã chọn: {rootParentObject.name}", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("⚠️ Kéo GameObject vào đây (object chứa parts-pos, vfx-part-pos...)", MessageType.Warning);
        }
        
        EditorGUILayout.Space();
        
        autoDetectBuildingId = EditorGUILayout.Toggle("🤖 Auto detect Building ID", autoDetectBuildingId);
        
        if (!autoDetectBuildingId)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Building ID:", GUILayout.Width(100));
            manualBuildingId = EditorGUILayout.TextField(manualBuildingId);
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.Space();
        
        // Custom Mapping
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        useCustomMapping = EditorGUILayout.Toggle("🔗 Custom Mapping Building ID", useCustomMapping);
        
        if (useCustomMapping)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("📍 Mapping ID:", GUILayout.Width(100));
            mappingBuildingId = EditorGUILayout.TextField(mappingBuildingId);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.HelpBox(
                "💡 Details sẽ có mappingBuildingId này khi scan", 
                MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("ℹ️ Mapping ID = Building ID", MessageType.Info);
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        
        GUI.enabled = rootParentObject != null;
        var scanButtonStyle = new GUIStyle(GUI.skin.button);
        scanButtonStyle.fontSize = 12;
        scanButtonStyle.fontStyle = FontStyle.Bold;
        
        if (GUILayout.Button($"🔍 SCAN Objects từ Scene", scanButtonStyle, GUILayout.Height(35)))
        {
            ScanObjectsFromScene();
        }
        GUI.enabled = true;
        
        if (hasScannedData)
        {
            EditorGUILayout.Space();
            int totalParts = scannedDataByBuilding.Sum(kvp => kvp.Value.Count);
            int totalDetails = scannedDataByBuilding.Sum(kvp => kvp.Value.Sum(p => p.details.Count));
            EditorGUILayout.HelpBox($"✅ Đã scan: {scannedDataByBuilding.Count} building(s), {totalParts} part(s), {totalDetails} detail(s)", MessageType.Info);
            
            foreach (var kvp in scannedDataByBuilding)
            {
                var firstDetail = kvp.Value.FirstOrDefault()?.details.FirstOrDefault();
                string mappingInfo = firstDetail != null ? $" → Mapping: {firstDetail.mappingBuildingId}" : "";
                EditorGUILayout.LabelField($"   📦 {kvp.Key}: {kvp.Value.Count} parts{mappingInfo}");
            }
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawScriptableObjectSection()
    {
        GUILayout.Label("💾 BƯỚC 2: ScriptableObject Data", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        targetScriptableObject = (ScriptableDataPartBuildings)EditorGUILayout.ObjectField(
            "📁 ScriptableObject",
            targetScriptableObject,
            typeof(ScriptableDataPartBuildings),
            false
        );
        
        if (targetScriptableObject != null)
        {
            int buildingCount = targetScriptableObject.GetBuildingCount();
            int totalParts = targetScriptableObject.GetTotalPartsCount();
            int totalDetails = targetScriptableObject.GetTotalDetailsCount();
            EditorGUILayout.HelpBox($"✅ Current: {buildingCount} building(s), {totalParts} part(s), {totalDetails} detail(s)", MessageType.Info);
            
            if (buildingCount > 0)
            {
                EditorGUILayout.LabelField("📊 Buildings:", EditorStyles.boldLabel);
                foreach (var building in targetScriptableObject.buildingDataList)
                {
                    EditorGUILayout.LabelField($"   🏢 {building.buildingId}: {building.PartsCount} parts, {building.DetailsCount} details");
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("⚠️ Chọn ScriptableDataPartBuildings để lưu data", MessageType.Warning);
        }
        
        EditorGUILayout.Space();
        
        GUI.enabled = hasScannedData && targetScriptableObject != null;
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("💾 Merge vào Building (Smart Update)", GUILayout.Height(30)))
        {
            SaveToScriptableObject(false);
        }
        
        if (GUILayout.Button("🔄 Replace Building", GUILayout.Height(30), GUILayout.Width(130)))
        {
            SaveToScriptableObject(true);
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.HelpBox(
            "💡 Merge: Smart update details\n" +
            "🔄 Replace: Xóa toàn bộ và lưu mới", 
            MessageType.Info);
        
        GUI.enabled = true;
        
        EditorGUILayout.EndVertical();
    }

    private void DrawDataPreview()
    {
        GUILayout.Label("👀 Preview Data", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        if (hasScannedData && scannedDataByBuilding.Count > 0)
        {
            EditorGUILayout.LabelField($"📊 Scanned Data:", EditorStyles.boldLabel);
            
            foreach (var buildingKvp in scannedDataByBuilding.Take(2))
            {
                EditorGUILayout.LabelField($"🏢 Building: {buildingKvp.Key}", EditorStyles.boldLabel);
                
                foreach (var part in buildingKvp.Value.Take(2))
                {
                    EditorGUILayout.LabelField($"   📦 Part: {part.name} ({part.details.Count} details)");
                    
                    foreach (var detail in part.details.Take(2))
                    {
                        string typeIcon = detail.type == PartBuildingType.building ? "🏗️" : "✨";
                        EditorGUILayout.LabelField($"      {typeIcon} [Map: {detail.mappingBuildingId}] @ ({detail.position_x:F2}, {detail.position_y:F2}, {detail.position_z:F2})");
                    }
                }
                
                EditorGUILayout.Space(3);
            }
        }
        else if (targetScriptableObject != null && targetScriptableObject.buildingDataList.Count > 0)
        {
            int totalBuildings = targetScriptableObject.GetBuildingCount();
            int totalDetails = targetScriptableObject.GetTotalDetailsCount();
            EditorGUILayout.LabelField($"📊 ScriptableObject: {totalBuildings} building(s), {totalDetails} detail(s)", EditorStyles.boldLabel);
            
            foreach (var building in targetScriptableObject.buildingDataList.Take(2))
            {
                EditorGUILayout.LabelField($"🏢 {building.buildingId}: {building.DetailsCount} details", EditorStyles.boldLabel);
                
                foreach (var part in building.parts.Take(2))
                {
                    EditorGUILayout.LabelField($"   📦 {part.name} ({part.details.Count} details)");
                    
                    foreach (var detail in part.details.Take(1))
                    {
                        string typeIcon = detail.type == PartBuildingType.building ? "🏗️" : "✨";
                        EditorGUILayout.LabelField($"      {typeIcon} [Map: {detail.mappingBuildingId}]");
                    }
                }
                
                EditorGUILayout.Space(3);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("📝 Chưa có data để preview", MessageType.Info);
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawActionButtons()
    {
        GUILayout.Label("⚡ Actions", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("👁️ Open ScriptableObject", GUILayout.Height(30)))
        {
            if (targetScriptableObject != null)
            {
                Selection.activeObject = targetScriptableObject;
                EditorGUIUtility.PingObject(targetScriptableObject);
            }
            else
            {
                EditorUtility.DisplayDialog("Info", "Chọn ScriptableObject trước!", "OK");
            }
        }
        
        if (GUILayout.Button("🔄 Clear Scanned Data", GUILayout.Height(30)))
        {
            ClearScannedData();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
    }

    // ============================================
    // CORE FUNCTIONS
    // ============================================

    private void ScanObjectsFromScene()
    {
        if (rootParentObject == null)
        {
            Debug.LogError("❌ Vui lòng chọn Root Object trước!");
            return;
        }

        scannedDataByBuilding.Clear();
        
        string buildingId = autoDetectBuildingId ? rootParentObject.name : manualBuildingId;
        string finalMappingId = useCustomMapping && !string.IsNullOrEmpty(mappingBuildingId) 
            ? mappingBuildingId 
            : buildingId;
        
        List<ScriptableDataPartBuildings.PosData> partsInBuilding = new();
        
        // Scan parts-pos folder
        Transform partsPos = rootParentObject.transform.Find("parts-pos");
        if (partsPos != null)
        {
            ScanChildren(partsPos, partsInBuilding, PartBuildingType.building, finalMappingId);
        }
        
        // Scan vfx-part-pos folder
        Transform vfxPartPos = rootParentObject.transform.Find("vfx-part-pos");
        if (vfxPartPos != null)
        {
            ScanChildren(vfxPartPos, partsInBuilding, PartBuildingType.vfx, finalMappingId);
        }
        
        scannedDataByBuilding[buildingId] = partsInBuilding;
        hasScannedData = true;
        
        int totalDetails = partsInBuilding.Sum(p => p.details.Count);
        
        Debug.Log($"✅ Scan hoàn tất!");
        Debug.Log($"   🏢 Building: {buildingId}");
        Debug.Log($"   📍 Mapping ID: {finalMappingId}");
        Debug.Log($"   📦 Total parts: {partsInBuilding.Count}");
        Debug.Log($"   📄 Total details: {totalDetails}");
    }

    private void ScanChildren(Transform parent, List<ScriptableDataPartBuildings.PosData> partsList, 
        PartBuildingType type, string mappingId)
    {
        foreach (Transform child in parent)
        {
            // Tìm part đã tồn tại hoặc tạo mới
            var existingPart = partsList.FirstOrDefault(p => p.name == child.name);
            
            if (existingPart == null)
            {
                existingPart = new ScriptableDataPartBuildings.PosData
                {
                    name = child.name,
                    details = new List<ScriptableDataPartBuildings.PosDataDetail>()
                };
                partsList.Add(existingPart);
            }
            
            // Thêm detail cho part này
            var detail = new ScriptableDataPartBuildings.PosDataDetail
            {
                mappingBuildingId = mappingId,
                type = type,
                localPosition = child.localPosition,
                rotation = child.localEulerAngles,
                scale = child.localScale
            };
            
            existingPart.details.Add(detail);
            
            // Recursive scan children
            if (child.childCount > 0)
            {
                ScanChildren(child, partsList, type, mappingId);
            }
        }
    }

    private void SaveToScriptableObject(bool replaceMode)
    {
        if (targetScriptableObject == null || !hasScannedData)
        {
            Debug.LogError("❌ Thiếu ScriptableObject hoặc chưa scan data!");
            return;
        }

        Undo.RecordObject(targetScriptableObject, "Update Part Position Data");
        
        foreach (var buildingKvp in scannedDataByBuilding)
        {
            string buildingId = buildingKvp.Key;
            List<ScriptableDataPartBuildings.PosData> parts = buildingKvp.Value;
            
            if (replaceMode)
            {
                targetScriptableObject.AddOrUpdateBuilding(buildingId, parts);
                Debug.Log($"✅ REPLACED building {buildingId}: {parts.Count} parts, {parts.Sum(p => p.details.Count)} details");
            }
            else
            {
                targetScriptableObject.MergePartsToBuilding(buildingId, parts);
                Debug.Log($"✅ MERGED building {buildingId}: {parts.Count} parts, {parts.Sum(p => p.details.Count)} details");
            }
        }
        
        EditorUtility.SetDirty(targetScriptableObject);
        AssetDatabase.SaveAssets();
        
        string modeText = replaceMode ? "REPLACED" : "MERGED";
        EditorUtility.DisplayDialog("Success", 
            $"Đã {modeText} data vào ScriptableObject!\n\n" +
            $"Buildings: {targetScriptableObject.GetBuildingCount()}\n" +
            $"Total Parts: {targetScriptableObject.GetTotalPartsCount()}\n" +
            $"Total Details: {targetScriptableObject.GetTotalDetailsCount()}", 
            "OK");
    }

    private void ClearScannedData()
    {
        if (EditorUtility.DisplayDialog("Clear Scanned Data", 
            "Bạn có chắc muốn xóa scanned data?\n(ScriptableObject data sẽ không bị ảnh hưởng)", 
            "Yes", "Cancel"))
        {
            scannedDataByBuilding.Clear();
            hasScannedData = false;
            Debug.Log("🗑️ Đã xóa scanned data");
        }
    }
}
