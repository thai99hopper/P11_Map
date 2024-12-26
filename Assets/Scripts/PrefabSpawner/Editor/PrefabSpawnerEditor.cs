using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrefabSpawner))]
public class PrefabSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Create prefab with position child"))
        {
            var spawner = (PrefabSpawner)target;
            var childCount = spawner.transform.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var childObj = spawner.transform.GetChild(i);
                var value = (GameObject)PrefabUtility.InstantiatePrefab(spawner.prefab);
                value.transform.SetParent(spawner.transform);
                value.transform.localScale = childObj.transform.localScale;
                value.transform.localPosition = childObj.transform.localPosition;
            }
        }
    }
}
