#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class AssetLoader
{
    public static GameObject LoadPrefab(string assetPath, string assetName)
    {
        var fullPath = System.IO.Path.Combine(assetPath, assetName + ".prefab");
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
        return prefab;
    }
    
    public static T LoadScriptableObject<T>(string assetPath, string assetName) where T : ScriptableObject
    {
        var fullPath = System.IO.Path.Combine(assetPath, assetName + ".asset");
        var scriptableObject = AssetDatabase.LoadAssetAtPath<T>(fullPath);
        return scriptableObject;
    }
    
    public static ScriptableObject LoadScriptableObject(string assetPath, string assetName)
    {
        var fullPath = System.IO.Path.Combine(assetPath, assetName + ".asset");
        var scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(fullPath);
        return scriptableObject;
    }
    
    public static ScriptableDataBuildings GetDataBuildings()
    {
        return LoadScriptableObject<ScriptableDataBuildings>("Assets/ScriptableObject", "EditorDataBuilding");
    }

    public static ScriptableCharacterOnMapData GetDataCharacterOnMap()
    {
        return LoadScriptableObject<ScriptableCharacterOnMapData>("Assets/Map/Scriptable", "EditorDataCharacterOnMap");
    }
    
}

#endif