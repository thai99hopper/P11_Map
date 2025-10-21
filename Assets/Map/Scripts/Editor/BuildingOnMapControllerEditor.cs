using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingOnMapController))]
public partial class BuildingOnMapControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        OnInspectorGUI_EasyFill();
        // NEW: Apply Positions from ScriptableObject section
        DrawPositionReferencesSection();
    }
}
