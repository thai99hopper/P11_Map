using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;

namespace Map.Scripts.Editor
{
    public class ToolExportCharacterOnMapConfigEditor : EditorWindow
    {
        private ScriptableCharacterOnMapData targetScriptableObject;
        private string exportFileName = "CatBuildingConfig";
        private string exportPath = "";
        private Vector2 scrollPosition;
        
        // Thêm option để sort dữ liệu
        private bool sortByBuildingId = true;
        
        [MenuItem("Tools/Export Character Map Config")]
        public static void ShowWindow()
        {
            GetWindow<ToolExportCharacterOnMapConfigEditor>("CSV Exporter");
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            GUILayout.Label("📊 Character Map CSV Exporter", EditorStyles.boldLabel);
            
            EditorGUILayout.Space();

            // Field để kéo ScriptableObject vào
            targetScriptableObject = (ScriptableCharacterOnMapData)EditorGUILayout.ObjectField(
                "📁 ScriptableObject Data",
                targetScriptableObject,
                typeof(ScriptableCharacterOnMapData),
                false
            );

            EditorGUILayout.Space();
            
            // Export settings
            DrawExportSettings();
            
            EditorGUILayout.Space();
            
            // Preview data
            DrawDataPreview();
            
            EditorGUILayout.Space();
            
            // Export buttons
            DrawExportButtons();
            
            EditorGUILayout.EndScrollView();
        }

        private void DrawExportSettings()
        {
            GUILayout.Label("⚙️ Export Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // File name
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("📝 File Name:", GUILayout.Width(80));
            exportFileName = EditorGUILayout.TextField(exportFileName);
            EditorGUILayout.LabelField(".csv", GUILayout.Width(30));
            EditorGUILayout.EndHorizontal();
            
            // Export path
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("📂 Export Path:", GUILayout.Width(80));
            EditorGUILayout.TextField(string.IsNullOrEmpty(exportPath) ? "Assets/" : exportPath);
            if (GUILayout.Button("📁 Browse", GUILayout.Width(70)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select Export Folder", "Assets", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Convert absolute path to relative path
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        exportPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        exportPath = selectedPath;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            
            // Thêm option sắp xếp theo building_id
            EditorGUILayout.Space();
            sortByBuildingId = EditorGUILayout.Toggle("🔄 Sắp xếp theo Building ID (nhóm cùng building)", sortByBuildingId);
            
            if (sortByBuildingId)
            {
                EditorGUILayout.HelpBox("✅ Các hàng có cùng building_id sẽ được nhóm lại với nhau trong CSV", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("ℹ️ Dữ liệu sẽ được export theo thứ tự hiện tại", MessageType.Info);
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawDataPreview()
        {
            GUILayout.Label("👀 Data Preview", EditorStyles.boldLabel);
            
            if (targetScriptableObject == null)
            {
                EditorGUILayout.HelpBox("⚠️ Vui lòng chọn ScriptableObject để preview data", MessageType.Warning);
                return;
            }

            if (targetScriptableObject.characterOnMapDataList == null || targetScriptableObject.characterOnMapDataList.Count == 0)
            {
                EditorGUILayout.HelpBox("📝 ScriptableObject không có data", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"📊 Tổng số records: {targetScriptableObject.characterOnMapDataList.Count}", EditorStyles.boldLabel);
            
            // Preview table (first 5 rows)
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Preview (5 rows đầu):", EditorStyles.boldLabel);
            
            var previewData = targetScriptableObject.characterOnMapDataList.Take(5);
            foreach (var data in previewData)
            {
                EditorGUILayout.LabelField($"[{data.building_id}] {data.cat_name} - {data.character_type} @ ({data.position_x:F1}, {data.position_y:F1}, {data.position_z:F1})");
            }
            
            if (targetScriptableObject.characterOnMapDataList.Count > 5)
            {
                EditorGUILayout.LabelField($"... và {targetScriptableObject.characterOnMapDataList.Count - 5} rows nữa");
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawExportButtons()
        {
            // Kiểm tra điều kiện export
            GUI.enabled = targetScriptableObject != null && 
                         targetScriptableObject.characterOnMapDataList != null && 
                         targetScriptableObject.characterOnMapDataList.Count > 0 &&
                         !string.IsNullOrEmpty(exportFileName);

            // Main export button
            var buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 14;
            buttonStyle.fontStyle = FontStyle.Bold;
            
            if (GUILayout.Button($"📤 Export {targetScriptableObject?.characterOnMapDataList?.Count ?? 0} Records to CSV", 
                buttonStyle, GUILayout.Height(40)))
            {
                ExportToCSV();
            }

            GUI.enabled = true;
            
            EditorGUILayout.Space();
            
            // Utility buttons
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("📋 Copy CSV to Clipboard", GUILayout.Height(25)))
            {
                CopyCSVToClipboard();
            }
            
            if (GUILayout.Button("👁️ Open Export Folder", GUILayout.Height(25)))
            {
                OpenExportFolder();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private string GenerateCSVContent()
        {
            if (targetScriptableObject == null || targetScriptableObject.characterOnMapDataList == null)
                return "";

            StringBuilder csv = new StringBuilder();
            
            // Header với comment như trong hình
            //csv.AppendLine("DATA RELY ON DEV TOOL");
            
            // Column headers
            csv.AppendLine("building_id,cat_name,character_type,position_x,position_y,position_z,rotation_x,rotation_y,rotation_z,scale_x,scale_y,scale_z");
            
            // Data rows
            var sortedData = sortByBuildingId 
                ? targetScriptableObject.characterOnMapDataList.OrderBy(data => data.building_id).ThenBy(data => data.cat_name).ToList()
                : targetScriptableObject.characterOnMapDataList;
            
            foreach (var data in sortedData)
            {
                csv.AppendLine($"{data.building_id},{data.cat_name},{data.character_type}," +
                              $"{data.position_x},{data.position_y},{data.position_z}," +
                              $"{data.rotation_x},{data.rotation_y},{data.rotation_z}," +
                              $"{data.scale_x},{data.scale_y},{data.scale_z}");
            }
            
            return csv.ToString();
        }

        private void ExportToCSV()
        {
            string csvContent = GenerateCSVContent();
            
            if (string.IsNullOrEmpty(csvContent))
            {
                Debug.LogError("❌ Không thể generate CSV content");
                return;
            }

            // Determine full export path
            string fullExportPath = string.IsNullOrEmpty(exportPath) ? "Assets/" : exportPath;
            string fullFileName = Path.Combine(fullExportPath, exportFileName + ".csv");
            
            try
            {
                // Ensure directory exists
                string directory = Path.GetDirectoryName(fullFileName);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                // Write file
                File.WriteAllText(fullFileName, csvContent, Encoding.UTF8);
                
                // Refresh asset database if in Assets folder
                if (fullFileName.StartsWith("Assets/"))
                {
                    AssetDatabase.Refresh();
                }
                
                Debug.Log($"✅ Đã export thành công: {fullFileName}");
                Debug.Log($"📊 Tổng số records: {targetScriptableObject.characterOnMapDataList.Count}");
                
                // Highlight file in Project window
                if (fullFileName.StartsWith("Assets/"))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<Object>(fullFileName);
                    if (asset != null)
                    {
                        EditorGUIUtility.PingObject(asset);
                    }
                }
                
                // Show success dialog
                EditorUtility.DisplayDialog("Export Success", 
                    $"CSV file đã được export thành công!\n\nFile: {fullFileName}\nRecords: {targetScriptableObject.characterOnMapDataList.Count}", 
                    "OK");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Lỗi khi export CSV: {ex.Message}");
                EditorUtility.DisplayDialog("Export Error", 
                    $"Có lỗi xảy ra khi export:\n{ex.Message}", 
                    "OK");
            }
        }

        private void CopyCSVToClipboard()
        {
            if (targetScriptableObject == null)
            {
                Debug.LogWarning("⚠️ Không có ScriptableObject để copy");
                return;
            }

            string csvContent = GenerateCSVContent();
            EditorGUIUtility.systemCopyBuffer = csvContent;
            
            Debug.Log($"📋 Đã copy {targetScriptableObject.characterOnMapDataList.Count} records vào clipboard");
            
            EditorUtility.DisplayDialog("Copy Success", 
                $"CSV content đã được copy vào clipboard!\n\nRecords: {targetScriptableObject.characterOnMapDataList.Count}", 
                "OK");
        }

        private void OpenExportFolder()
        {
            string folderPath = string.IsNullOrEmpty(exportPath) ? "Assets/" : exportPath;
            
            if (folderPath.StartsWith("Assets/"))
            {
                // Convert to absolute path
                folderPath = Application.dataPath + folderPath.Substring(6);
            }
            
            if (Directory.Exists(folderPath))
            {
                EditorUtility.RevealInFinder(folderPath);
            }
            else
            {
                Debug.LogWarning($"⚠️ Folder không tồn tại: {folderPath}");
            }
        }
    }
}
