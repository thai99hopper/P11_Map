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
}

#endif