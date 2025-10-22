using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;

namespace Map.Scripts.Editor
{
    public class ToolExportPartPosOnmapConfigEditor : EditorWindow
    {
        private ScriptableDataPartBuildings targetScriptableObject;
        private string exportFileName = "PartBuildingPosConfig";
        private string exportPath = "";
        private Vector2 scrollPosition;

        // Options
        private bool sortByBuildingId = true;

        [MenuItem("Tools/Export Part Buildings Config")]
        public static void ShowWindow()
        {
            GetWindow<ToolExportPartPosOnmapConfigEditor>("Part Buildings CSV Exporter");
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label("📦 Part Buildings CSV Exporter", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            // Field để kéo ScriptableObject vào
            targetScriptableObject = (ScriptableDataPartBuildings)EditorGUILayout.ObjectField(
                "📁 ScriptableObject Data",
                targetScriptableObject,
                typeof(ScriptableDataPartBuildings),
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
            if (GUILayout.Button("📁 Browse", GUILayout.Width(80)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select Export Folder", "Assets", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
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

            // Sort option
            EditorGUILayout.Space();
            sortByBuildingId = EditorGUILayout.Toggle("🔀 Sắp xếp theo Building ID", sortByBuildingId);

            if (sortByBuildingId)
            {
                EditorGUILayout.HelpBox("✅ Dữ liệu sẽ được nhóm theo building_id", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("ℹ️ Dữ liệu sẽ export theo thứ tự hiện tại", MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawDataPreview()
        {
            GUILayout.Label("👁️ Data Preview", EditorStyles.boldLabel);

            if (targetScriptableObject == null)
            {
                EditorGUILayout.HelpBox("⚠️ Vui lòng chọn ScriptableObject để preview data", MessageType.Warning);
                return;
            }

            if (targetScriptableObject.buildingDataList == null || targetScriptableObject.buildingDataList.Count == 0)
            {
                EditorGUILayout.HelpBox("📝 ScriptableObject không có data", MessageType.Info);
                return;
            }

            // Statistics
            int totalBuildings = targetScriptableObject.GetBuildingCount();
            int totalParts = targetScriptableObject.GetTotalPartsCount();
            int totalDetails = targetScriptableObject.GetTotalDetailsCount();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"📊 Tổng số Buildings: {totalBuildings}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"📦 Tổng số Parts: {totalParts}");
            EditorGUILayout.LabelField($"📋 Tổng số Details (rows sẽ export): {totalDetails}");
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Preview first few records
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Preview (5 rows đầu):", EditorStyles.boldLabel);

            int count = 0;
            foreach (var building in targetScriptableObject.buildingDataList)
            {
                foreach (var part in building.parts)
                {
                    foreach (var detail in part.details)
                    {
                        if (count >= 5) break;

                        EditorGUILayout.LabelField(
                            $"[{building.buildingId}] {part.name} → {detail.mappingBuildingId} ({detail.type}) @ ({detail.position_x:F1}, {detail.position_y:F1}, {detail.position_z:F1})"
                        );
                        count++;
                    }
                    if (count >= 5) break;
                }
                if (count >= 5) break;
            }

            if (totalDetails > 5)
            {
                EditorGUILayout.LabelField($"... và {totalDetails - 5} rows nữa");
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawExportButtons()
        {
            // Check conditions
            GUI.enabled = targetScriptableObject != null &&
                         targetScriptableObject.buildingDataList != null &&
                         targetScriptableObject.buildingDataList.Count > 0 &&
                         !string.IsNullOrEmpty(exportFileName);

            // Main export button
            var buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };

            int totalDetails = targetScriptableObject != null ? targetScriptableObject.GetTotalDetailsCount() : 0;

            if (GUILayout.Button($"📤 Export {totalDetails} Records to CSV", buttonStyle, GUILayout.Height(40)))
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
            if (targetScriptableObject == null || targetScriptableObject.buildingDataList == null)
                return "";

            StringBuilder csv = new StringBuilder();

            // Column headers - giống format trong ảnh
            csv.AppendLine("building_id,building_mapping,part_info,building_type,position_x,position_y,position_z,rotation_x,rotation_y,rotation_z,scale_x,scale_y,scale_z");

            // Flatten data structure
            var allRows = targetScriptableObject.buildingDataList
                .SelectMany(building => building.parts
                    .SelectMany(part => part.details
                        .Select(detail => new
                        {
                            buildingId = building.buildingId,
                            partName = part.name,
                            mappingId = detail.mappingBuildingId,
                            type = detail.type.ToString(),
                            detail.position_x,
                            detail.position_y,
                            detail.position_z,
                            detail.rotation_x,
                            detail.rotation_y,
                            detail.rotation_z,
                            detail.scale_x,
                            detail.scale_y,
                            detail.scale_z
                        })
                    )
                );

            // Sort if needed
            if (sortByBuildingId)
            {
                allRows = allRows.OrderBy(r => r.buildingId).ThenBy(r => r.partName);
            }

            // Write data rows
            foreach (var row in allRows)
            {
                csv.AppendLine($"{row.buildingId},{row.mappingId},{row.partName},{row.type}," +
                              $"{row.position_x},{row.position_y},{row.position_z}," +
                              $"{row.rotation_x},{row.rotation_y},{row.rotation_z}," +
                              $"{row.scale_x},{row.scale_y},{row.scale_z}");
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
                if (fullFileName.StartsWith("Assets"))
                {
                    AssetDatabase.Refresh();
                }

                int totalRecords = targetScriptableObject.GetTotalDetailsCount();

                Debug.Log($"✅ Đã export thành công: {fullFileName}");
                Debug.Log($"📊 Tổng số records: {totalRecords}");

                // Highlight file in Project window
                if (fullFileName.StartsWith("Assets"))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<Object>(fullFileName);
                    if (asset != null)
                    {
                        EditorGUIUtility.PingObject(asset);
                    }
                }

                // Show success dialog
                EditorUtility.DisplayDialog("Export Success",
                    $"CSV file đã được export thành công!\n\nFile: {fullFileName}\nRecords: {totalRecords}",
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

            int totalRecords = targetScriptableObject.GetTotalDetailsCount();

            Debug.Log($"📋 Đã copy {totalRecords} records vào clipboard");

            EditorUtility.DisplayDialog("Copy Success",
                $"CSV content đã được copy vào clipboard!\n\nRecords: {totalRecords}",
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
