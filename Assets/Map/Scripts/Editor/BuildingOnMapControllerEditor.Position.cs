using UnityEditor;
using UnityEngine;
using System.Linq;

public partial class BuildingOnMapControllerEditor
{
    private ScriptableDataPartBuildings positionData;
    private string inputBuildingId = "";
    private string inputMappingBuildingId = "";
    
    private void DrawPositionReferencesSection()
    {
        EditorGUILayout.Space(10);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("📍 Apply Positions from ScriptableObject", EditorStyles.boldLabel);
        
        // ScriptableObject field
        positionData = (ScriptableDataPartBuildings)EditorGUILayout.ObjectField(
            "Position Data",
            positionData,
            typeof(ScriptableDataPartBuildings),
            false
        );
        
        if (positionData != null)
        {
            EditorGUILayout.HelpBox(
                $"✅ Data: {positionData.GetBuildingCount()} building(s), " +
                $"{positionData.GetTotalPartsCount()} part(s), " +
                $"{positionData.GetTotalDetailsCount()} detail(s)", 
                MessageType.Info);
            
            EditorGUILayout.Space(5);
            
            // Input fields cho Building ID và Mapping ID
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("🔑 Filter Settings:", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Building ID:", GUILayout.Width(100));
            inputBuildingId = EditorGUILayout.TextField(inputBuildingId);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mapping ID:", GUILayout.Width(100));
            inputMappingBuildingId = EditorGUILayout.TextField(inputMappingBuildingId);
            EditorGUILayout.EndHorizontal();
            
            // Quick fill buttons
            EditorGUILayout.BeginHorizontal();
            BuildingOnMapController controller = (BuildingOnMapController)target;
            
            if (GUILayout.Button($"📝 Use GameObject Name ({controller.gameObject.name})", GUILayout.Height(20)))
            {
                inputBuildingId = controller.gameObject.name;
                inputMappingBuildingId = controller.gameObject.name;
            }
            
            if (GUILayout.Button("🔄 Auto Fill", GUILayout.Height(20)))
            {
                inputBuildingId = controller.gameObject.name;
                inputMappingBuildingId = controller.gameObject.name;
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(3);
            
            // Validation
            bool isValid = !string.IsNullOrEmpty(inputBuildingId) && !string.IsNullOrEmpty(inputMappingBuildingId);
            
            if (!isValid)
            {
                EditorGUILayout.HelpBox("⚠️ Vui lòng nhập Building ID và Mapping ID!", MessageType.Warning);
            }
            else
            {
                // Tìm building data theo inputBuildingId
                var buildingData = positionData.buildingDataList.FirstOrDefault(b => b.buildingId == inputBuildingId);
                
                if (buildingData != null)
                {
                    // Đếm details có mappingBuildingId phù hợp
                    int matchingDetails = buildingData.parts
                        .SelectMany(p => p.details)
                        .Count(d => d.mappingBuildingId == inputMappingBuildingId);
                    
                    EditorGUILayout.HelpBox(
                        $"🏢 Found data for building: {inputBuildingId}\n" +
                        $"📍 Mapping ID: {inputMappingBuildingId}\n" +
                        $"✅ Matching details: {matchingDetails}", 
                        MessageType.Info);
                    
                    if (matchingDetails == 0)
                    {
                        EditorGUILayout.HelpBox(
                            $"⚠️ Không tìm thấy details nào với mapping ID: {inputMappingBuildingId}\n" +
                            $"Available mappings: {string.Join(", ", buildingData.parts.SelectMany(p => p.details).Select(d => d.mappingBuildingId).Distinct())}", 
                            MessageType.Warning);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        $"⚠️ Không tìm thấy building: {inputBuildingId}\n" +
                        $"Available buildings: {string.Join(", ", positionData.buildingDataList.Select(b => b.buildingId))}", 
                        MessageType.Warning);
                }
            }
            
            EditorGUILayout.Space(5);
            
            // Apply button - chỉ enable khi valid
            GUI.enabled = isValid;
            
            var applyButtonStyle = new GUIStyle(GUI.skin.button);
            applyButtonStyle.fontSize = 12;
            applyButtonStyle.fontStyle = FontStyle.Bold;
            
            if (GUILayout.Button("✨ Apply Positions to Scene Objects", applyButtonStyle, GUILayout.Height(35)))
            {
                ApplyPositionsToScene(controller, inputBuildingId, inputMappingBuildingId);
            }
            
            GUI.enabled = true;
            
            EditorGUILayout.Space(3);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("🔄 Reset to Original", GUILayout.Height(25)))
            {
                ResetToOriginalPositions(controller);
            }
            
            if (GUILayout.Button("💾 Save Current as Original", GUILayout.Height(25)))
            {
                SaveCurrentAsOriginal(controller);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox(
                "ℹ️ Kéo ScriptableDataPartBuildings vào đây để apply positions", 
                MessageType.Info);
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void ApplyPositionsToScene(BuildingOnMapController controller, string buildingId, string mappingId)
    {
        if (positionData == null)
        {
            Debug.LogError("❌ Position data null!");
            return;
        }
        
        // Tìm building data
        var buildingData = positionData.buildingDataList.FirstOrDefault(b => b.buildingId == buildingId);
        
        if (buildingData == null || buildingData.parts == null)
        {
            Debug.LogError($"❌ Không tìm thấy building data cho: {buildingId}");
            return;
        }
        
        Transform rootTransform = controller.transform;
        int appliedCount = 0;
        int notFoundCount = 0;
        int skippedCount = 0;
        int buildingTypeCount = 0;
        int vfxTypeCount = 0;
        
        // Lưu original positions trước khi apply (để có thể reset)
        if (!EditorPrefs.HasKey($"{controller.GetInstanceID()}_HasOriginalPos"))
        {
            SaveCurrentAsOriginal(controller);
        }
        
        Debug.Log($"🔍 ===== BẮT ĐẦU APPLY POSITIONS =====");
        Debug.Log($"🏢 Building ID: {buildingId}");
        Debug.Log($"📍 Mapping ID: {mappingId}");
        Debug.Log($"📦 Tổng số parts trong data: {buildingData.parts.Count}");
        
        // IMPORTANT: Đếm trước để biết có bao nhiêu VFX trong data
        int totalBuildingPartsInData = buildingData.parts.Count(p => 
            p.details != null && p.details.Any(d => d.mappingBuildingId == mappingId && d.type == PartBuildingType.building));
        int totalVfxPartsInData = buildingData.parts.Count(p => 
            p.details != null && p.details.Any(d => d.mappingBuildingId == mappingId && d.type == PartBuildingType.vfx));
        
        Debug.Log($"📊 Parts có trong data với mapping '{mappingId}':");
        Debug.Log($"   🏢 Building parts: {totalBuildingPartsInData}");
        Debug.Log($"   ✨ VFX parts: {totalVfxPartsInData}");
        
        if (totalVfxPartsInData == 0)
        {
            Debug.LogWarning($"⚠️ CẢNH BÁO: Không có VFX parts nào trong data với mapping ID '{mappingId}'!");
            Debug.LogWarning($"💡 Kiểm tra ScriptableObject xem có parts nào có type=vfx và mappingBuildingId='{mappingId}' không");
        }
        
        // List để track các parts theo type
        System.Collections.Generic.List<string> buildingParts = new System.Collections.Generic.List<string>();
        System.Collections.Generic.List<string> vfxParts = new System.Collections.Generic.List<string>();
        System.Collections.Generic.List<string> vfxPartsNotFound = new System.Collections.Generic.List<string>();
        
        foreach (var part in buildingData.parts)
        {
            if (part.details == null || part.details.Count == 0)
            {
                Debug.LogWarning($"⚠️ Part '{part.name}' không có details, skip!");
                continue;
            }
            
            Debug.Log($"\n🔍 Đang xử lý part: {part.name}");
            Debug.Log($"   📊 Số details: {part.details.Count}");
            
            // Log tất cả details để debug
            foreach (var d in part.details)
            {
                Debug.Log($"   📋 Detail: MappingID={d.mappingBuildingId}, Type={d.type}, Pos={d.localPosition}");
            }
            
            // ⭐ FIX: Lấy TẤT CẢ details với mappingBuildingId phù hợp (không chỉ first)
            var matchingDetails = part.details.Where(d => d.mappingBuildingId == mappingId).ToList();
            
            if (matchingDetails.Count == 0)
            {
                skippedCount++;
                Debug.LogWarning($"   ⏭️ Không tìm thấy detail với mapping ID: {mappingId}");
                Debug.LogWarning($"   Available mappings: {string.Join(", ", part.details.Select(d => $"{d.mappingBuildingId}({d.type})"))}");
                continue;
            }
            
            Debug.Log($"   ✅ Found {matchingDetails.Count} detail(s) with mapping ID: {mappingId}");
            
            // Loop qua TẤT CẢ matching details (có thể có cả building VÀ vfx)
            foreach (var detail in matchingDetails)
            {
                // Log detail info
                Debug.Log($"   ✅ Processing detail: Type={detail.type}, Pos={detail.localPosition}, Rot={detail.rotation}, Scale={detail.scale}");
                
                // Track theo type
                if (detail.type == PartBuildingType.building)
                    buildingParts.Add(part.name);
                else if (detail.type == PartBuildingType.vfx)
                    vfxParts.Add(part.name);
                
                // Tìm GameObject trong scene theo tên VÀ TYPE
                Transform targetTransform = FindChildRecursive(rootTransform, part.name, detail.type);
                
                if (targetTransform != null)
                {
                    Undo.RecordObject(targetTransform, "Apply Position from ScriptableObject");
                    
                    // Log vị trí cũ
                    Debug.Log($"   📍 OLD → Pos: {targetTransform.localPosition}, Rot: {targetTransform.localEulerAngles}, Scale: {targetTransform.localScale}");
                    
                    // Apply position, rotation, scale
                    targetTransform.localPosition = detail.localPosition;
                    targetTransform.localEulerAngles = detail.rotation;
                    targetTransform.localScale = detail.scale;
                    
                    // Log vị trí mới
                    Debug.Log($"   📍 NEW → Pos: {targetTransform.localPosition}, Rot: {targetTransform.localEulerAngles}, Scale: {targetTransform.localScale}");
                    
                    appliedCount++;
                    
                    // Đếm theo type
                    if (detail.type == PartBuildingType.building)
                        buildingTypeCount++;
                    else if (detail.type == PartBuildingType.vfx)
                        vfxTypeCount++;
                    
                    // Kiểm tra parent của object
                    string parentName = targetTransform.parent != null ? targetTransform.parent.name : "NULL";
                    Debug.Log($"   🌳 Parent: {parentName}");
                    Debug.Log($"   ✅ Applied successfully! (Type: {detail.type})");
                }
                else
                {
                    notFoundCount++;
                    Debug.LogWarning($"   ❌ Không tìm thấy GameObject trong scene: {part.name} (Type: {detail.type})");
                    Debug.LogWarning($"   💡 Tip: Kiểm tra tên object trong Hierarchy phải giống với '{part.name}'");
                    
                    // Track VFX not found riêng
                    if (detail.type == PartBuildingType.vfx)
                    {
                        vfxPartsNotFound.Add(part.name);
                    }
                }
            }
        }
        
        Debug.Log($"\n🎉 ===== APPLY POSITIONS HOÀN TẤT =====");
        Debug.Log($"📊 THỐNG KÊ:");
        Debug.Log($"   ✔️ Applied: {appliedCount} objects");
        Debug.Log($"   🏢 Building type: {buildingTypeCount} objects");
        Debug.Log($"   ✨ VFX type: {vfxTypeCount} objects");
        
        if (buildingParts.Count > 0)
        {
            Debug.Log($"\n🏢 Building parts found in data: {string.Join(", ", buildingParts)}");
        }
        if (vfxParts.Count > 0)
        {
            Debug.Log($"✨ VFX parts found in data: {string.Join(", ", vfxParts)}");
        }
        
        // IMPORTANT: Cảnh báo về VFX
        if (totalVfxPartsInData > 0 && vfxTypeCount == 0)
        {
            Debug.LogError($"\n❌❌❌ VẤN ĐỀ VFX: ❌❌❌");
            Debug.LogError($"📊 Có {totalVfxPartsInData} VFX parts trong ScriptableObject");
            Debug.LogError($"❌ Nhưng {vfxTypeCount} VFX được apply!");
            
            if (vfxPartsNotFound.Count > 0)
            {
                Debug.LogError($"🔍 VFX parts KHÔNG TÌM THẤY trong Scene:");
                foreach (var vfxName in vfxPartsNotFound)
                {
                    Debug.LogError($"   ❌ {vfxName}");
                }
                Debug.LogError($"💡 GIẢI PHÁP: Kiểm tra tên các VFX objects trong Hierarchy (vfx-part-pos)");
                Debug.LogError($"   Tên phải khớp CHÍNH XÁC với tên trong ScriptableObject!");
            }
            
            if (vfxParts.Count > vfxPartsNotFound.Count)
            {
                int vfxSkipped = vfxParts.Count - vfxPartsNotFound.Count;
                Debug.LogError($"⚠️ Có {vfxSkipped} VFX parts bị skip vì sai mapping ID");
            }
        }
        
        if (notFoundCount > 0)
        {
            Debug.LogWarning($"\n   ⚠️ Not found in scene: {notFoundCount} objects");
            Debug.LogWarning($"   💡 Những objects này có trong ScriptableObject nhưng không có trong Scene");
        }
        if (skippedCount > 0)
        {
            Debug.LogWarning($"   ⏭️ Skipped (wrong mapping): {skippedCount} objects");
            Debug.LogWarning($"   💡 Những parts này không có detail với mapping ID: {mappingId}");
        }
        
        string resultMessage = $"Đã apply positions!\n\n" +
            $"Building ID: {buildingId}\n" +
            $"Mapping ID: {mappingId}\n\n" +
            $"✔️ Applied: {appliedCount} objects\n" +
            $"🏢 Building type: {buildingTypeCount}\n" +
            $"✨ VFX type: {vfxTypeCount}\n";
        
        if (notFoundCount > 0)
        {
            resultMessage += $"⚠️ Not found: {notFoundCount} objects\n";
        }
        if (skippedCount > 0)
        {
            resultMessage += $"⏭️ Skipped: {skippedCount} objects\n";
        }
        
        // Cảnh báo đặc biệt cho VFX
        if (totalVfxPartsInData > 0 && vfxTypeCount == 0)
        {
            resultMessage += $"\n❌ VẤN ĐỀ: Có {totalVfxPartsInData} VFX trong data nhưng 0 được apply!\n" +
                           $"Kiểm tra Console logs để biết chi tiết.";
        }
        
        EditorUtility.DisplayDialog("Apply Success", resultMessage, "OK");
        
        // Mark scene dirty
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(controller.gameObject.scene);
    }
    
    private Transform FindChildRecursive(Transform parent, string childName, PartBuildingType type)
    {
        Debug.Log($"🔍 [FindChildRecursive] Searching for: {childName}, Type: {type}");
        
        // Tìm theo TYPE để tránh conflict
        if (type == PartBuildingType.building)
        {
            // Tìm BUILDING trong parts-pos
            Transform partsPos = parent.Find("parts-pos");
            if (partsPos != null)
            {
                Transform found = FindInChildren(partsPos, childName);
                if (found != null)
                {
                    Debug.Log($"🔍 ✅ Found BUILDING in parts-pos: {childName}");
                    return found;
                }
            }
            
            // Fallback: Tìm trong children trực tiếp (nếu không có parts-pos)
            Transform directChild = parent.Find(childName);
            if (directChild != null)
            {
                Debug.Log($"🔍 ✅ Found direct child (BUILDING): {childName}");
                return directChild;
            }
        }
        else if (type == PartBuildingType.vfx)
        {
            // Tìm VFX trong vfx-part-pos
            Transform vfxPartPos = parent.Find("vfx-part-pos") ?? parent.Find("vfx-parts-pos");
            if (vfxPartPos != null)
            {
                Transform found = FindInChildren(vfxPartPos, childName);
                if (found != null)
                {
                    Debug.Log($"🔍 ✅ Found VFX in vfx-part-pos: {childName}");
                    return found;
                }
            }
            else
            {
                Debug.LogWarning($"🔍 ⚠️ vfx-part-pos folder NOT FOUND! Cannot search for VFX: {childName}");
            }
            
            // Fallback: Tìm trong children trực tiếp (nếu không có vfx-part-pos)
            Transform directChild = parent.Find(childName);
            if (directChild != null)
            {
                Debug.Log($"🔍 ✅ Found direct child (VFX): {childName}");
                return directChild;
            }
        }
        
        Debug.LogWarning($"🔍 ❌ Not found anywhere: {childName} (Type: {type})");
        return null;
    }
    
    private Transform FindInChildren(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            
            Transform found = FindInChildren(child, name);
            if (found != null)
                return found;
        }
        
        return null;
    }
    
    private void SaveCurrentAsOriginal(BuildingOnMapController controller)
    {
        Transform rootTransform = controller.transform;
        int savedCount = 0;
        
        // Save parts-pos
        Transform partsPos = rootTransform.Find("parts-pos");
        if (partsPos != null)
        {
            savedCount += SaveTransformRecursive(partsPos, "parts-pos");
        }
        
        // Save vfx-part-pos
        Transform vfxPartPos = rootTransform.Find("vfx-part-pos");
        if (vfxPartPos != null)
        {
            savedCount += SaveTransformRecursive(vfxPartPos, "vfx-part-pos");
        }
        
        EditorPrefs.SetBool($"{controller.GetInstanceID()}_HasOriginalPos", true);
        
        Debug.Log($"💾 Đã lưu original positions: {savedCount} objects");
        
        EditorUtility.DisplayDialog("Save Success", 
            $"Đã lưu original positions!\n\n{savedCount} objects", 
            "OK");
    }
    
    private int SaveTransformRecursive(Transform parent, string prefix)
    {
        int count = 0;
        
        foreach (Transform child in parent)
        {
            string key = $"{prefix}/{GetTransformPath(child)}";
            
            // Save position
            EditorPrefs.SetString($"{key}_pos", JsonUtility.ToJson(child.localPosition));
            EditorPrefs.SetString($"{key}_rot", JsonUtility.ToJson(child.localEulerAngles));
            EditorPrefs.SetString($"{key}_scale", JsonUtility.ToJson(child.localScale));
            
            count++;
            
            if (child.childCount > 0)
            {
                count += SaveTransformRecursive(child, prefix);
            }
        }
        
        return count;
    }
    
    private void ResetToOriginalPositions(BuildingOnMapController controller)
    {
        if (!EditorPrefs.HasKey($"{controller.GetInstanceID()}_HasOriginalPos"))
        {
            EditorUtility.DisplayDialog("No Original Data", 
                "Chưa có original positions được lưu!\n\n" +
                "Click 'Save Current as Original' trước để lưu.", 
                "OK");
            return;
        }
        
        Transform rootTransform = controller.transform;
        int restoredCount = 0;
        
        // Restore parts-pos
        Transform partsPos = rootTransform.Find("parts-pos");
        if (partsPos != null)
        {
            restoredCount += RestoreTransformRecursive(partsPos, "parts-pos");
        }
        
        // Restore vfx-part-pos
        Transform vfxPartPos = rootTransform.Find("vfx-part-pos");
        if (vfxPartPos != null)
        {
            restoredCount += RestoreTransformRecursive(vfxPartPos, "vfx-part-pos");
        }
        
        Debug.Log($"🔄 Đã reset về original positions: {restoredCount} objects");
        
        EditorUtility.DisplayDialog("Reset Success", 
            $"Đã reset về original positions!\n\n{restoredCount} objects", 
            "OK");
        
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(controller.gameObject.scene);
    }
    
    private int RestoreTransformRecursive(Transform parent, string prefix)
    {
        int count = 0;
        
        foreach (Transform child in parent)
        {
            string key = $"{prefix}/{GetTransformPath(child)}";
            
            if (EditorPrefs.HasKey($"{key}_pos"))
            {
                Undo.RecordObject(child, "Reset to Original Position");
                
                child.localPosition = JsonUtility.FromJson<Vector3>(EditorPrefs.GetString($"{key}_pos"));
                child.localEulerAngles = JsonUtility.FromJson<Vector3>(EditorPrefs.GetString($"{key}_rot"));
                child.localScale = JsonUtility.FromJson<Vector3>(EditorPrefs.GetString($"{key}_scale"));
                
                count++;
            }
            
            if (child.childCount > 0)
            {
                count += RestoreTransformRecursive(child, prefix);
            }
        }
        
        return count;
    }
    
    private string GetTransformPath(Transform transform)
    {
        string path = transform.name;
        Transform parent = transform.parent;
        
        while (parent != null && parent.name != "parts-pos" && parent.name != "vfx-part-pos")
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return path;
    }
}
