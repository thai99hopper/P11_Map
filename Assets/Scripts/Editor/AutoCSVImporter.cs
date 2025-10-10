using UnityEditor;
using UnityEngine;
using System.IO;

public class AutoCSVImporter : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
        string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets)
        {
            if (asset.EndsWith("UpgradeBuildingResourceConfig.csv"))
            {
                Debug.Log($"CSV file updated: {asset}");
                EditorUtility.DisplayDialog("CSV Updated", 
                    $"Building CSV file has been updated!\nFile: {Path.GetFileName(asset)}\n\nDo you want to open the Building Data Importer?", 
                    "Open Importer", "Later");
                
                if (EditorUtility.DisplayDialog("Open Importer", "Open Building Data Importer now?", "Yes", "No"))
                {
                    TransferDataBuildingEditor.ShowWindow();
                }
            }
        }
    }
}
