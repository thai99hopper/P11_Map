using UnityEditor;
using UnityEngine;

public partial class BuildingOnMapControllerEditor
{
    private void OnInspectorGUI_EasyFill()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space(10);
        
        BuildingOnMapController controller = (BuildingOnMapController)target;
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("🔧 Auto Fill Position References", EditorStyles.boldLabel);
        
        EditorGUILayout.HelpBox(
            "Tự động scan và fill:\n" +
            "• lPosParts: từ parts-pos/*\n" +
            "• lPosVfxs: từ vfx-part-pos/*", 
            MessageType.Info);
        
        var buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 12;
        buttonStyle.fontStyle = FontStyle.Bold;
        
        if (GUILayout.Button("🔄 Auto Fill Position References", buttonStyle, GUILayout.Height(35)))
        {
            AutoFillPositionReferences(controller);
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void AutoFillPositionReferences(BuildingOnMapController controller)
    {
        Undo.RecordObject(controller, "Auto Fill Position References");
        
        // Clear existing lists
        var lPosPartsField = typeof(BuildingOnMapController).GetField("lPosParts", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var lPosVfxsField = typeof(BuildingOnMapController).GetField("lPosVfxs", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (lPosPartsField == null || lPosVfxsField == null)
        {
            Debug.LogError("❌ Không tìm thấy fields lPosParts hoặc lPosVfxs!");
            return;
        }
        
        var lPosParts = lPosPartsField.GetValue(controller) as System.Collections.IList;
        var lPosVfxs = lPosVfxsField.GetValue(controller) as System.Collections.IList;
        
        if (lPosParts == null || lPosVfxs == null)
        {
            Debug.LogError("❌ Lists không khởi tạo!");
            return;
        }
        
        lPosParts.Clear();
        lPosVfxs.Clear();
        
        Transform rootTransform = controller.transform;
        int partsCount = 0;
        int vfxsCount = 0;
        
        // Scan parts-pos folder
        Transform partsPos = rootTransform.Find("parts-pos");
        if (partsPos != null)
        {
            partsCount = ScanAndAddChildren(partsPos, lPosParts);
        }
        else
        {
            Debug.LogWarning("⚠️ Không tìm thấy folder 'parts-pos'");
        }
        
        // Scan vfx-part-pos folder
        Transform vfxPartPos = rootTransform.Find("vfx-part-pos") ?? rootTransform.Find("vfx-parts-pos");
        if (vfxPartPos != null)
        {
            vfxsCount = ScanAndAddChildren(vfxPartPos, lPosVfxs);
        }
        else
        {
            Debug.LogWarning("⚠️ Không tìm thấy folder 'vfx-part-pos'");
        }
        
        EditorUtility.SetDirty(controller);
        
        Debug.Log($"✅ Auto Fill hoàn tất!");
        Debug.Log($"   📦 lPosParts: {partsCount} objects");
        Debug.Log($"   ✨ lPosVfxs: {vfxsCount} objects");
        
        EditorUtility.DisplayDialog("Auto Fill Success", 
            $"Đã fill position references!\n\n" +
            $"• lPosParts: {partsCount} objects\n" +
            $"• lPosVfxs: {vfxsCount} objects", 
            "OK");
    }
    
    private int ScanAndAddChildren(Transform parent, System.Collections.IList targetList)
    {
        int count = 0;
        
        foreach (Transform child in parent)
        {
            // Tạo ObjectPos mới
            var objectPosType = typeof(BuildingOnMapController).GetNestedType("ObjectPos");
            var objectPos = System.Activator.CreateInstance(objectPosType);
            
            // Set name
            var nameField = objectPosType.GetField("name");
            nameField.SetValue(objectPos, child.name);
            
            // Set transform
            var transfField = objectPosType.GetField("transf");
            transfField.SetValue(objectPos, child);
            
            targetList.Add(objectPos);
            count++;
            
            // Recursive scan children
            if (child.childCount > 0)
            {
                count += ScanAndAddChildren(child, targetList);
            }
        }
        
        return count;
    }
}
