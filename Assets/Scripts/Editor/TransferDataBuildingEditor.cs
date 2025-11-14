using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class TransferDataBuildingEditor : EditorWindow
{
    private string csvFilePath = "MappingEditor.csv";
    private ScriptableDataBuildings targetScriptableObject;
    private Vector2 scrollPosition;
    private List<Dictionary<string, string>> csvData;
    private bool showPreview = false;
    private bool autoUpdate = true;
    
    [MenuItem("Map/ScriptableObject/Update Building On Map")]
    public static void ShowWindow()
    {
        var window = GetWindow<TransferDataBuildingEditor>("Building Data Importer");
        window.minSize = new Vector2(600, 400);
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Building Data CSV Importer", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Bước 1: Chọn file CSV
        EditorGUILayout.LabelField("Bước 1: Chọn file CSV", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        csvFilePath = EditorGUILayout.TextField("CSV File Path:", csvFilePath);
        if (GUILayout.Button("Browse", GUILayout.Width(70)))
        {
            string path = EditorUtility.OpenFilePanel("Select CSV File", "Assets", "csv");
            if (!string.IsNullOrEmpty(path))
            {
                csvFilePath = path;
            }
        }
        EditorGUILayout.EndHorizontal();
        
        // Hiển thị trạng thái file
        if (File.Exists(csvFilePath))
        {
            EditorGUILayout.HelpBox($"✓ File found: {Path.GetFileName(csvFilePath)}", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("✗ File not found!", MessageType.Warning);
        }
        
        // Bước 2: Chọn ScriptableObject đích
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Bước 2: Chọn ScriptableObject đích", EditorStyles.boldLabel);
        targetScriptableObject = (ScriptableDataBuildings)EditorGUILayout.ObjectField(
            "Target ScriptableObject:", 
            targetScriptableObject, 
            typeof(ScriptableDataBuildings), 
            false
        );
        
        autoUpdate = EditorGUILayout.Toggle("Auto Update after Import", autoUpdate);
        
        // Bước 3: Load và Preview dữ liệu
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Bước 3: Load và Preview", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        
        GUI.enabled = File.Exists(csvFilePath);
        if (GUILayout.Button("Load CSV Data"))
        {
            LoadCSVData();
        }
        GUI.enabled = true;
        
        if (GUILayout.Button("Create New ScriptableObject"))
        {
            CreateNewScriptableObject();
        }
        if (GUILayout.Button("Get Config"))
        {
            Application.OpenURL("https://docs.google.com/spreadsheets/d/1_mlYIcyUeQBvM3uXl-Ww9lTVisApHwldjB7CPTYMjkI/edit?pli=1&gid=749142426#gid=749142426");
        }
        EditorGUILayout.EndHorizontal();
        
        // Hiển thị preview
        if (csvData != null && csvData.Count > 0)
        {
            showPreview = EditorGUILayout.Foldout(showPreview, $"Preview Data ({csvData.Count} entries)");
            if (showPreview)
            {
                DrawPreview();
            }
            
            // Bước 4: Import dữ liệu
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Bước 4: Import dữ liệu", EditorStyles.boldLabel);
            GUI.enabled = targetScriptableObject != null;
            if (GUILayout.Button("Import Data to ScriptableObject", GUILayout.Height(40)))
            {
                ImportDataToScriptableObject();
            }
            GUI.enabled = true;
        }
        
        // Quick Actions
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
        GUI.enabled = File.Exists(csvFilePath) && targetScriptableObject != null;
        if (GUILayout.Button("One-Click Import (Load + Import)", GUILayout.Height(30)))
        {
            LoadCSVData();
            if (csvData != null && csvData.Count > 0)
            {
                ImportDataToScriptableObject();
            }
        }
        GUI.enabled = true;
    }
    
    private void LoadCSVData()
    {
        if (string.IsNullOrEmpty(csvFilePath) || !File.Exists(csvFilePath))
        {
            EditorUtility.DisplayDialog("Error", "CSV file not found!", "OK");
            return;
        }
        
        try
        {
            csvData = CSVReader.Read(csvFilePath);
            Debug.Log($"Successfully loaded {csvData.Count} entries from CSV");
            
            if (csvData.Count > 0)
            {
                Debug.Log("CSV Headers detected: " + string.Join(", ", csvData[0].Keys));
            }
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to load CSV: {e.Message}", "OK");
            Debug.LogError($"CSV Load Error: {e}");
        }
    }
    
    private void CreateNewScriptableObject()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Create ScriptableDataBuildings",
            "BuildingData",
            "asset",
            "Save ScriptableObject"
        );
        
        if (!string.IsNullOrEmpty(path))
        {
            var newAsset = CreateInstance<ScriptableDataBuildings>();
            newAsset.buildings = new List<ScriptableDataBuildings.BuildingData>();
            AssetDatabase.CreateAsset(newAsset, path);
            AssetDatabase.SaveAssets();
            targetScriptableObject = newAsset;
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newAsset;
            Debug.Log($"Created new ScriptableObject at: {path}");
        }
    }
    
    private void DrawPreview()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
        
        int previewCount = Mathf.Min(3, csvData.Count);
        for (int i = 0; i < previewCount; i++)
        {
            var entry = csvData[i];
            EditorGUILayout.LabelField($"Building {i + 1}:", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.LabelField("Building Name:", GetValueFromEntry(entry, "buldingname"));
            EditorGUILayout.LabelField("Mapping Building:", GetValueFromEntry(entry, "mappingbuilding"));
            EditorGUILayout.LabelField("Character Resource ID:", GetValueFromEntry(entry, "mappingbuildingcharacterresourceid"));
            EditorGUILayout.LabelField("Model Path 1:", GetValueFromEntry(entry, "modelloadpath01"));
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
        
        if (csvData.Count > previewCount)
        {
            EditorGUILayout.LabelField($"... and {csvData.Count - previewCount} more entries");
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    private void ImportDataToScriptableObject()
    {
        if (targetScriptableObject == null || csvData == null)
        {
            EditorUtility.DisplayDialog("Error", "Missing target ScriptableObject or CSV data!", "OK");
            return;
        }
        
        try
        {
            // Ensure buildings list is initialized
            if (targetScriptableObject.buildings == null)
            {
                targetScriptableObject.buildings = new List<ScriptableDataBuildings.BuildingData>();
            }
            
            // Clear existing data
            targetScriptableObject.buildings.Clear();
            
            int successCount = 0;
            foreach (var entry in csvData)
            {
                var buildingData = new ScriptableDataBuildings.BuildingData();
                
                // Initialize modelLoadPaths first
                buildingData.modelLoadPaths = new List<ScriptableDataBuildings.ModelLoadPath>();
                
                // Map CSV columns to BuildingData properties with corrected header names
                buildingData.buildingId = GetValueFromEntry(entry, "buldingname");
                buildingData.buildingIdMapping = GetValueFromEntry(entry, "mappingbuilding");
                buildingData.nameTextLocalizeMapping = GetValueFromEntry(entry, "mappingnametextlocalize");
                buildingData.nameTextTooltipLocalizeMapping = GetValueFromEntry(entry, "mappingnametexttooltip");
                
                // Parse character resource ID
                string resourceIdStr = GetValueFromEntry(entry, "mappingbuildingcharacterresourceid");
                if (int.TryParse(resourceIdStr, out int resourceId))
                {
                    buildingData.buildingCharacterResourceIdMapping = resourceId;
                }
                
                // Model Path 1
                AddModelPath(buildingData, entry, "modelloadpath01", "orderinbuilding01");
                
                // Model Path 2
                AddModelPath(buildingData, entry, "modelloadpatheditor02", "orderinbuilding02");
                
                // Model Path 3
                AddModelPath(buildingData, entry, "modelloadpatheditor03", "orderinbuilding03");
                
                buildingData.buildingIdSpine = GetValueFromEntry(entry, "buildingnamespine");
                buildingData.buildingIdSpineMapping = GetValueFromEntry(entry, "mappingspinename");
                
                targetScriptableObject.buildings.Add(buildingData);
                successCount++;
            }
            
            // Mark as dirty and save
            EditorUtility.SetDirty(targetScriptableObject);
            
            if (autoUpdate)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log($"Auto-updated ScriptableObject: {targetScriptableObject.name}");
            }
            
            EditorUtility.DisplayDialog("Success", 
                $"Successfully imported {successCount} building entries!\n" +
                $"Target: {targetScriptableObject.name}\n" +
                (autoUpdate ? "File automatically saved!" : "Remember to save the file!"), "OK");
            
            Debug.Log($"Import completed! {successCount} buildings imported to {targetScriptableObject.name}");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Import failed: {e.Message}", "OK");
            Debug.LogError($"Import Error: {e}");
        }
    }
    
    private void AddModelPath(ScriptableDataBuildings.BuildingData buildingData, 
        Dictionary<string, string> entry, string pathKey, string orderKey)
    {
        string modelPath = GetValueFromEntry(entry, pathKey);
        if (!string.IsNullOrEmpty(modelPath) && modelPath != "none")
        {
            var modelLoad = new ScriptableDataBuildings.ModelLoadPath();
            modelLoad.modelPath = modelPath;
            
            string orderStr = GetValueFromEntry(entry, orderKey);
            if (int.TryParse(orderStr, out int order))
            {
                modelLoad.orderBuilding = order;
            }
            
            buildingData.modelLoadPaths.Add(modelLoad);
        }
    }
    
    private string GetValueFromEntry(Dictionary<string, string> entry, string key)
    {
        return entry.ContainsKey(key) ? entry[key] : "";
    }
}
