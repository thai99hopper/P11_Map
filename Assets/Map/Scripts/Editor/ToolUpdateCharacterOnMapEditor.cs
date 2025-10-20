using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Map.Scripts.Editor
{
    public class ToolUpdateCharacterOnMapEditor : EditorWindow
    {
        private ScriptableCharacterOnMapData targetScriptableObject;
        
        // Thêm field để chọn McOnMapController
        private McOnMapController targetMcController;
        
        // Thay đổi từ selectedObjects sang danh sách objects có thể edit riêng biệt
        [System.Serializable]
        public class EditableObject
        {
            public GameObject gameObject;
            public CharacterOnMapType characterType;
            public string catName;
            public string buildingId;
            public bool isExpanded;
        }
        
        private List<EditableObject> editableObjects = new List<EditableObject>();
        
        // Thêm các biến để lưu giá trị input mặc định
        private string defaultCatName = "";
        private string defaultBuildingId = "";
        private CharacterOnMapType defaultCharacterType = CharacterOnMapType.idle;
        
        // Thêm biến để điều khiển chế độ
        private bool useAutoDetection = true;
        
        private Vector2 scrollPosition;
        private Vector2 objectListScrollPosition;

        // Dictionary để map keywords với character types
        private Dictionary<string, CharacterOnMapType> keywordToTypeMap = new Dictionary<string, CharacterOnMapType>
        {
            {"idle", CharacterOnMapType.idle},
            {"move", CharacterOnMapType.move},
            {"initialize", CharacterOnMapType.initialize},
            {"collider", CharacterOnMapType.box_collider},
            {"box_collider", CharacterOnMapType.box_collider},
            {"pos_1", CharacterOnMapType.move_emo_pos_1},
            {"pos_2", CharacterOnMapType.move_emo_pos_2},
            {"pos_3", CharacterOnMapType.move_emo_pos_3},
            {"pos_4", CharacterOnMapType.move_emo_pos_4},
            {"pos_5", CharacterOnMapType.move_emo_pos_5},
            {"move_pos_1", CharacterOnMapType.move_pos_1},
            {"move_pos_2", CharacterOnMapType.move_pos_2},
            {"move_pos_3", CharacterOnMapType.move_pos_3},
            {"move_pos_4", CharacterOnMapType.move_pos_4},
            {"move_pos_5", CharacterOnMapType.move_pos_5},
        };

        [MenuItem("Tools/Update Character On Map")]
        public static void ShowWindow()
        {
            GetWindow<ToolUpdateCharacterOnMapEditor>("Character Map Updater");
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            GUILayout.Label("🎯 Character Map Tool - Đơn Giản & Dễ Dùng", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            // Field để kéo ScriptableObject vào
            targetScriptableObject = (ScriptableCharacterOnMapData)EditorGUILayout.ObjectField(
                "📁 ScriptableObject Data",
                targetScriptableObject,
                typeof(ScriptableCharacterOnMapData),
                false
            );

            // Thêm field để kéo McOnMapController vào
            EditorGUILayout.Space();
            targetMcController = (McOnMapController)EditorGUILayout.ObjectField(
                "🎮 McOnMapController (Optional)",
                targetMcController,
                typeof(McOnMapController),
                true
            );
            
            // Hiển thị thông tin về McOnMapController
            if (targetMcController != null)
            {
                EditorGUILayout.HelpBox($"✅ Sẽ cập nhật characterPositions trong {targetMcController.name}", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("⚠️ McOnMapController chưa được gán!\n💡 Kéo McOnMapController vào để tự động cập nhật characterPositions", MessageType.Warning);
            }

            EditorGUILayout.Space();
            
            // Chỉ giữ lại setting quan trọng nhất
            useAutoDetection = EditorGUILayout.Toggle("🤖 Tự động phát hiện loại từ tên object", useAutoDetection);
            
            if (!useAutoDetection)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("🎛️ Loại mặc định:", GUILayout.Width(100));
                defaultCharacterType = (CharacterOnMapType)EditorGUILayout.EnumPopup(defaultCharacterType);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            
            // Thêm phần Global Settings
            DrawGlobalSettings();

            EditorGUILayout.Space();

            // Danh sách objects đơn giản
            DrawSimpleObjectList();

            EditorGUILayout.Space();
            
            // Buttons chính đơn giản
            DrawMainButtons();
            
            EditorGUILayout.EndScrollView();
        }

        private void DrawGlobalSettings()
        {
            GUILayout.Label("🌍 Cài Đặt Chung - Áp Dụng Cho Tất Cả Objects", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // Cat Name chung
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("🏷️ Cat Name mặc định:", GUILayout.Width(130));
            defaultCatName = EditorGUILayout.TextField(defaultCatName);
            if (GUILayout.Button("📋 Áp dụng cho tất cả", GUILayout.Width(120)))
            {
                ApplyGlobalCatName();
            }
            EditorGUILayout.EndHorizontal();
            
            // Building ID chung
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("🏢 Building ID mặc định:", GUILayout.Width(130));
            defaultBuildingId = EditorGUILayout.TextField(defaultBuildingId);
            if (GUILayout.Button("📋 Áp dụng cho tất cả", GUILayout.Width(120)))
            {
                ApplyGlobalBuildingId();
            }
            EditorGUILayout.EndHorizontal();
            
            // Thêm nút kiểm tra dữ liệu
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = targetScriptableObject != null && (!string.IsNullOrEmpty(defaultCatName) || !string.IsNullOrEmpty(defaultBuildingId));
            if (GUILayout.Button("🔍 Kiểm tra dữ liệu đã tồn tại", GUILayout.Height(30)))
            {
                CheckExistingData();
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            
            // Character Type chung (khi tắt auto-detection)
            if (!useAutoDetection)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("🎛️ Character Type mặc định:", GUILayout.Width(130));
                defaultCharacterType = (CharacterOnMapType)EditorGUILayout.EnumPopup(defaultCharacterType);
                if (GUILayout.Button("📋 Áp dụng cho tất cả", GUILayout.Width(120)))
                {
                    ApplyGlobalCharacterType();
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Space();
            
            // Buttons hữu ích
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🔄 Reset tất cả về tên GameObject"))
            {
                ResetAllToGameObjectNames();
            }
            if (GUILayout.Button("🎯 Tự động phát hiện lại tất cả"))
            {
                AutoDetectAllObjects();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.HelpBox("💡 Mẹo: Chỉnh sửa các giá trị trên và nhấn 'Áp dụng cho tất cả' để tất cả objects trong danh sách sẽ sử dụng giá trị này!", MessageType.Info);
        }
        
        private void ApplyGlobalCatName()
        {
            if (string.IsNullOrEmpty(defaultCatName))
            {
                Debug.LogWarning("⚠️ Cat Name trống!");
                return;
            }
            
            foreach (var obj in editableObjects)
            {
                obj.catName = defaultCatName;
            }
            Debug.Log($"✅ Đã áp dụng Cat Name '{defaultCatName}' cho {editableObjects.Count} objects");
        }
        
        private void ApplyGlobalBuildingId()
        {
            foreach (var obj in editableObjects)
            {
                obj.buildingId = defaultBuildingId;
            }
            Debug.Log($"✅ Đã áp dụng Building ID '{defaultBuildingId}' cho {editableObjects.Count} objects");
        }
        
        private void ApplyGlobalCharacterType()
        {
            foreach (var obj in editableObjects)
            {
                obj.characterType = defaultCharacterType;
            }
            Debug.Log($"✅ Đã áp dụng Character Type '{defaultCharacterType}' cho {editableObjects.Count} objects");
        }
        
        private void ResetAllToGameObjectNames()
        {
            foreach (var obj in editableObjects)
            {
                obj.catName = obj.gameObject.name;
                obj.buildingId = "";
                if (useAutoDetection)
                {
                    obj.characterType = DetectCharacterType(obj.gameObject.name);
                }
            }
            Debug.Log($"🔄 Đã reset {editableObjects.Count} objects về tên GameObject gốc");
        }
        
        private void DrawSimpleObjectList()
        {
            GUILayout.Label("📋 Danh Sách Objects", EditorStyles.boldLabel);
            
            // Thêm objects từ selection
            if (Selection.gameObjects.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox($"Có {Selection.gameObjects.Length} object(s) được chọn", MessageType.Info);
                if (GUILayout.Button("➕ Thêm vào danh sách", GUILayout.Width(120)))
                {
                    AddSelectedObjects();
                }
                EditorGUILayout.EndHorizontal();
            }
            
            // Hiển thị số lượng objects trong danh sách
            if (editableObjects.Count > 0)
            {
                EditorGUILayout.LabelField($"📊 Có {editableObjects.Count} objects trong danh sách", EditorStyles.boldLabel);
                
                // Nút xóa tất cả
                if (GUILayout.Button("🗑️ Xóa tất cả khỏi danh sách", GUILayout.Height(25)))
                {
                    editableObjects.Clear();
                }
                
                EditorGUILayout.Space();
            }
            else
            {
                EditorGUILayout.HelpBox("📝 Danh sách trống.\n\n💡 Cách sử dụng:\n1. Chọn objects trong Scene\n2. Nhấn 'Thêm vào danh sách'\n3. Chỉnh sửa Character Type nếu c��n\n4. Nhấn 'Cập Nhật Dữ Liệu'", MessageType.Info);
                return;
            }

            objectListScrollPosition = EditorGUILayout.BeginScrollView(objectListScrollPosition, GUILayout.Height(250));
            
            for (int i = 0; i < editableObjects.Count; i++)
            {
                var editableObject = editableObjects[i];
                
                if (editableObject.gameObject == null)
                {
                    editableObjects.RemoveAt(i);
                    i--;
                    continue;
                }
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                // Header đơn giản với tên object và character type
                EditorGUILayout.BeginHorizontal();
                
                // Icon và tên object
                GUIContent objectContent = EditorGUIUtility.ObjectContent(editableObject.gameObject, typeof(GameObject));
                EditorGUILayout.LabelField(objectContent, GUILayout.Width(200));
                
                // Character Type dropdown với màu sắc
                var typeColor = GetTypeColor(editableObject.characterType);
                var originalColor = GUI.backgroundColor;
                GUI.backgroundColor = typeColor;
                editableObject.characterType = (CharacterOnMapType)EditorGUILayout.EnumPopup(editableObject.characterType, GUILayout.Width(120));
                GUI.backgroundColor = originalColor;
                
                // Nút xóa
                if (GUILayout.Button("���", GUILayout.Width(30)))
                {
                    editableObjects.RemoveAt(i);
                    i--;
                    break;
                }
                
                EditorGUILayout.EndHorizontal();
                
                // Hiển thị thông tin nhanh
                var pos = editableObject.gameObject.transform.position;
                EditorGUILayout.LabelField($"📍 Position: ({pos.x:F1}, {pos.y:F1}, {pos.z:F1})", EditorStyles.miniLabel);
                
                // Details section đơn giản
                editableObject.isExpanded = EditorGUILayout.Foldout(editableObject.isExpanded, "⚙️ Tùy chỉnh thêm");
                if (editableObject.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    
                    // Cat Name
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("🏷️ Cat Name:", GUILayout.Width(80));
                    editableObject.catName = EditorGUILayout.TextField(editableObject.catName);
                    EditorGUILayout.EndHorizontal();
                    
                    // Building ID
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("🏢 Building ID:", GUILayout.Width(80));
                    editableObject.buildingId = EditorGUILayout.TextField(editableObject.buildingId);
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(3);
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void AddSelectedObjects()
        {
            int addedCount = 0;
            foreach (var obj in Selection.gameObjects)
            {
                // Kiểm tra xem object đã có trong danh sách chưa
                if (!editableObjects.Any(e => e.gameObject == obj))
                {
                    editableObjects.Add(new EditableObject
                    {
                        gameObject = obj,
                        characterType = useAutoDetection ? DetectCharacterType(obj.name) : defaultCharacterType,
                        catName = string.IsNullOrEmpty(defaultCatName) ? obj.name : defaultCatName,
                        buildingId = defaultBuildingId, // Tự động áp dụng Building ID từ Global Settings
                        isExpanded = false
                    });
                    addedCount++;
                }
            }
            
            if (addedCount > 0)
            {
                Debug.Log($"✅ Đã thêm {addedCount} object(s) vào danh sách với Global Settings");
                if (!string.IsNullOrEmpty(defaultCatName))
                {
                    Debug.Log($"   🏷️ Cat Name: '{defaultCatName}'");
                }
                if (!string.IsNullOrEmpty(defaultBuildingId))
                {
                    Debug.Log($"   🏢 Building ID: '{defaultBuildingId}'");
                }
            }
            else
            {
                Debug.Log("ℹ️ Tất cả objects đã có trong danh sách rồi");
            }
        }

        private void DrawMainButtons()
        {
            EditorGUILayout.Space();
            
            // Hiển thị trạng thái
            if (targetScriptableObject == null)
            {
                EditorGUILayout.HelpBox("⚠️ Vui lòng kéo ScriptableObject vào field bên trên trước", MessageType.Warning);
                return;
            }
            
            if (editableObjects.Count == 0)
            {
                EditorGUILayout.HelpBox("ℹ️ Chưa có objects nào trong danh sách", MessageType.Info);
                return;
            }
            
            // Nút cập nhật chính - to và rõ ràng
            var buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 14;
            buttonStyle.fontStyle = FontStyle.Bold;
            
            string buttonText = $"🚀 Cập Nhật {editableObjects.Count} Objects";
            if (targetMcController != null)
            {
                buttonText += " + McController";
            }
            
            if (GUILayout.Button(buttonText, buttonStyle, GUILayout.Height(40)))
            {
                UpdateCharacterData();
                
                // Cập nhật McOnMapController nếu có
                if (targetMcController != null)
                {
                    UpdateMcOnMapController();
                }
            }

            EditorGUILayout.Space();

            // Buttons phụ nhỏ hơn
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("🔄 Tự động phát hiện lại tất cả", GUILayout.Height(25)))
            {
                AutoDetectAllObjects();
            }
            
            // Thêm nút riêng cho McController
            GUI.enabled = targetMcController != null && editableObjects.Count > 0;
            if (GUILayout.Button("🎮 Chỉ cập nhật McController", GUILayout.Height(25)))
            {
                UpdateMcOnMapController();
            }
            GUI.enabled = true;
            
            if (GUILayout.Button("🗑️ Xóa dữ liệu ScriptableObject", GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("⚠️ Xác nhận xóa",
                    "Bạn có chắc muốn xóa toàn bộ dữ liệu trong ScriptableObject?", "Xóa", "Hủy"))
                {
                    ClearAllData();
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        // Thêm method để kiểm tra dữ liệu đã tồn tại
        private void CheckExistingData()
        {
            if (targetScriptableObject == null)
            {
                Debug.LogWarning("⚠️ Không có ScriptableObject để kiểm tra");
                return;
            }

            bool hasCatName = !string.IsNullOrEmpty(defaultCatName);
            bool hasBuildingId = !string.IsNullOrEmpty(defaultBuildingId);
            
            if (!hasCatName && !hasBuildingId)
            {
                Debug.LogWarning("⚠️ Vui lòng nhập Cat Name hoặc Building ID để kiểm tra");
                return;
            }

            var results = new List<ScriptableCharacterOnMapData.CharacterOnMapData>();
            
            // Tìm kiếm dựa trên các tiêu chí
            foreach (var data in targetScriptableObject.characterOnMapDataList)
            {
                bool matchCatName = !hasCatName || data.cat_name == defaultCatName;
                bool matchBuildingId = !hasBuildingId || data.building_id == defaultBuildingId;
                
                if (matchCatName && matchBuildingId)
                {
                    results.Add(data);
                }
            }

            // Hiển thị kết quả
            if (results.Count == 0)
            {
                string searchCriteria = "";
                if (hasCatName && hasBuildingId)
                {
                    searchCriteria = $"Cat Name: '{defaultCatName}' VÀ Building ID: '{defaultBuildingId}'";
                }
                else if (hasCatName)
                {
                    searchCriteria = $"Cat Name: '{defaultCatName}'";
                }
                else if (hasBuildingId)
                {
                    searchCriteria = $"Building ID: '{defaultBuildingId}'";
                }
                
                Debug.Log($"❌ KHÔNG TÌM THẤY dữ liệu với {searchCriteria}");
                EditorUtility.DisplayDialog("🔍 Kết quả kiểm tra", 
                    $"❌ Không tìm thấy dữ liệu nào với:\n{searchCriteria}\n\n✅ An toàn để thêm mới!", 
                    "OK");
            }
            else
            {
                string searchCriteria = "";
                if (hasCatName && hasBuildingId)
                {
                    searchCriteria = $"Cat Name: '{defaultCatName}' VÀ Building ID: '{defaultBuildingId}'";
                }
                else if (hasCatName)
                {
                    searchCriteria = $"Cat Name: '{defaultCatName}'";
                }
                else if (hasBuildingId)
                {
                    searchCriteria = $"Building ID: '{defaultBuildingId}'";
                }

                Debug.Log($"✅ TÌM THẤY {results.Count} dữ liệu với {searchCriteria}:");
                
                string detailMessage = $"🔍 Tìm thấy {results.Count} dữ liệu v��i:\n{searchCriteria}\n\n";
                
                for (int i = 0; i < results.Count; i++)
                {
                    var data = results[i];
                    Debug.Log($"   {i + 1}. [{data.building_id}|{data.cat_name}|{data.character_type}] tại ({data.position_x:F1}, {data.position_y:F1}, {data.position_z:F1})");
                    
                    detailMessage += $"{i + 1}. Building ID: '{data.building_id}'\n";
                    detailMessage += $"   Cat Name: '{data.cat_name}'\n";
                    detailMessage += $"   Character Type: {data.character_type}\n";
                    detailMessage += $"   Position: ({data.position_x:F1}, {data.position_y:F1}, {data.position_z:F1})\n\n";
                    
                    if (i >= 4) // Giới hạn hiển thị trong dialog để không quá dài
                    {
                        detailMessage += $"... và {results.Count - 5} dữ liệu khác (xem Console để biết chi tiết)";
                        break;
                    }
                }

                EditorUtility.DisplayDialog("🔍 Kết quả kiểm tra", detailMessage, "OK");
            }

            Debug.Log($"📊 Tổng số dữ liệu trong ScriptableObject: {targetScriptableObject.characterOnMapDataList.Count}");
        }
        
        // Thêm method để cập nhật McOnMapController
        private void UpdateMcOnMapController()
        {
            if (targetMcController == null)
            {
                Debug.LogWarning("⚠️ Không có McOnMapController để cập nhật");
                return;
            }

            Undo.RecordObject(targetMcController, "Update McOnMapController CharacterPositions");

            // Sử dụng reflection để truy cập characterPositions (vì nó là private)
            var characterPositionsField = typeof(McOnMapController).GetField("characterPositions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (characterPositionsField == null)
            {
                Debug.LogError("❌ Không tìm thấy field characterPositions trong McOnMapController");
                return;
            }

            var characterPositions = (List<McOnMapController.CharacterPosition>)characterPositionsField.GetValue(targetMcController);
            if (characterPositions == null)
            {
                characterPositions = new List<McOnMapController.CharacterPosition>();
                characterPositionsField.SetValue(targetMcController, characterPositions);
            }

            int addedCount = 0;
            int updatedCount = 0;

            foreach (var editableObject in editableObjects)
            {
                // Tìm existing position với cùng type
                var existingPos = characterPositions.FirstOrDefault(cp => cp.type == editableObject.characterType);
                
                if (existingPos != null)
                {
                    // Cập nhật existing position
                    existingPos.pos = editableObject.gameObject.transform;
                    updatedCount++;
                    Debug.Log($"🔄 Updated existing CharacterPosition: {editableObject.characterType} -> {editableObject.gameObject.name}");
                }
                else
                {
                    // Thêm mới
                    var newCharacterPosition = new McOnMapController.CharacterPosition
                    {
                        pos = editableObject.gameObject.transform,
                        type = editableObject.characterType
                    };
                    
                    characterPositions.Add(newCharacterPosition);
                    addedCount++;
                    Debug.Log($"➕ Added new CharacterPosition: {editableObject.characterType} -> {editableObject.gameObject.name}");
                }
            }

            // Cập nhật lại field
            characterPositionsField.SetValue(targetMcController, characterPositions);
            
            EditorUtility.SetDirty(targetMcController);
            
            Debug.Log($"✅ McOnMapController cập nhật: {addedCount} thêm mới + {updatedCount} cập nhật = {addedCount + updatedCount} positions");
            Debug.Log($"📊 Tổng CharacterPositions: {characterPositions.Count}");
        }
        
        private void AutoDetectAllObjects()
        {
            foreach (var obj in editableObjects)
            {
                obj.characterType = DetectCharacterType(obj.gameObject.name);
                obj.catName = obj.gameObject.name;
            }
            Debug.Log($"🔄 Đã tự động phát hiện lại cho {editableObjects.Count} objects");
        }
        
        private Color GetTypeColor(CharacterOnMapType type)
        {
            switch (type)
            {
                case CharacterOnMapType.idle: return Color.green;
                case CharacterOnMapType.move: return Color.blue;
                case CharacterOnMapType.box_collider: return Color.red;
                case CharacterOnMapType.initialize: return Color.yellow;
                default: return Color.white;
            }
        }

        private CharacterOnMapType DetectCharacterType(string objectName)
        {
            var lowerName = objectName.ToLower();
            
            // Ưu tiên detect theo thứ tự từ cụ thể đến chung chung
            var orderedKeywords = keywordToTypeMap.OrderByDescending(x => x.Key.Length);
            
            foreach (var kvp in orderedKeywords)
            {
                if (lowerName.Contains(kvp.Key))
                {
                    return kvp.Value;
                }
            }
            
            return CharacterOnMapType.idle; // Giá trị mặc định
        }

        private void UpdateCharacterData()
        {
            Undo.RecordObject(targetScriptableObject, "Update Character Data");

            int overrideCount = 0;
            int newCount = 0;

            foreach (var editableObject in editableObjects)
            {
                // Kiểm tra xem object đã có trong list dựa trên 3 key properties
                var existingData = targetScriptableObject.characterOnMapDataList
                    .FirstOrDefault(x => x.building_id == editableObject.buildingId && 
                                       x.cat_name == editableObject.catName && 
                                       x.character_type == editableObject.characterType);

                if (existingData != null)
                {
                    // Override data đã có với transform mới
                    Debug.Log($"🔄 Override existing: [{existingData.building_id}|{existingData.cat_name}|{existingData.character_type}] với position mới ({editableObject.gameObject.transform.position.x:F1}, {editableObject.gameObject.transform.position.y:F1}, {editableObject.gameObject.transform.position.z:F1})");
                    UpdateTransformData(existingData, editableObject.gameObject.transform);
                    overrideCount++;
                }
                else
                {
                    // Tạo mới data khi không tìm thấy match với 3 key properties
                    var newData = new ScriptableCharacterOnMapData.CharacterOnMapData
                    {
                        cat_name = editableObject.catName,
                        building_id = editableObject.buildingId,
                        character_type = editableObject.characterType
                    };

                    UpdateTransformData(newData, editableObject.gameObject.transform);
                    targetScriptableObject.characterOnMapDataList.Add(newData);
                    
                    Debug.Log($"➕ Created new: [{newData.building_id}|{newData.cat_name}|{newData.character_type}] tại position ({editableObject.gameObject.transform.position.x:F1}, {editableObject.gameObject.transform.position.y:F1}, {editableObject.gameObject.transform.position.z:F1})");
                    newCount++;
                }
            }

            EditorUtility.SetDirty(targetScriptableObject);
            AssetDatabase.SaveAssets();

            Debug.Log($"✅ Kết quả: {overrideCount} override + {newCount} tạo mới = {editableObjects.Count} objects đã xử lý");
            Debug.Log($"📊 Tổng data trong ScriptableObject: {targetScriptableObject.characterOnMapDataList.Count} elements");
        }

        private void UpdateTransformData(ScriptableCharacterOnMapData.CharacterOnMapData data, Transform transform)
        {
            // Sử dụng world position (transform.position) cho tất cả coordinates
            // Lý do: Character spawning cần vị trí tuyệt đối trên map, không phụ thuộc parent
            data.position_x = transform.position.x;
            data.position_y = transform.position.y;
            data.position_z = transform.position.z;

            // World rotation
            data.rotation_x = transform.eulerAngles.x;
            data.rotation_y = transform.eulerAngles.y;
            data.rotation_z = transform.eulerAngles.z;

            // Local scale (vì scale thường là tương đối)
            data.scale_x = transform.localScale.x;
            data.scale_y = transform.localScale.y;
            data.scale_z = transform.localScale.z;
        }

        private void ClearAllData()
        {
            Undo.RecordObject(targetScriptableObject, "Clear Character Data");
            targetScriptableObject.characterOnMapDataList.Clear();
            EditorUtility.SetDirty(targetScriptableObject);
            AssetDatabase.SaveAssets();
            Debug.Log("Đã xóa toàn bộ dữ liệu");
        }
    }
}
