using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TransferPosition))]
public class TrasnferPositionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Create prefab with position child"))
        {
            var trasnfer = (TransferPosition)target;
            var childCount = trasnfer.transform.childCount;
            var worldPos = trasnfer.transform.position;
            for (var i = 0; i < childCount; i++)
            {
                var childObj = trasnfer.transform.GetChild(i);
                var posChange = childObj.transform.localPosition;
                posChange += worldPos;
                childObj.transform.localPosition = posChange;
            }
        }
    }
}
