#if UNITY_EDITOR
using AssetUsageDetectorNamespace;

#if USE_SPINE
using Spine.Unity;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ObjectCleanerEditor : EditorWindow
{
    public const string FBX_FILE = ".fbx";
    public const string META_FILE = ".meta";
    public const string SHADER_GRAPH_STR = "Shader Graph";

    private EditorUIElement_pickFolder folderOuput = new EditorUIElement_pickFolder("output");


    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/object cleaner")]
    static void Init()
    {
        var window = (ObjectCleanerEditor)GetWindow(typeof(ObjectCleanerEditor));
        window.titleContent = new GUIContent("object cleaner");
        window.Show();
    }

    private void OnGUI()
    {
        folderOuput.Draw();
#if USE_SPINE
        if (GUILayout.Button("clean as asset skeleton data "))
        {
            var fileDelete = 0;
            var path = folderOuput.PickedPath;
            var materials = GetAtPath<SkeletonDataAsset>(path);
            foreach (var material in materials)
            {
                int linkReferenceCount = 0;
                var assetDetector = new AssetUsageDetector();
                var result = assetDetector.Run(new AssetUsageDetector.Parameters()
                {
                    objectsToSearch = new Object[] { material },
                    lazySceneSearch = false,
                    searchRefactoring = (searchMatch) =>
                    {
                        linkReferenceCount++;
                    },
                    showDetailedProgressBar = false,
#if ASSET_USAGE_ADDRESSABLES
            addressablesSupport = true,
#endif
                });
                fileDelete += CleanAsNormal(material, linkReferenceCount) ? 1 : 0;
            }
            EditorUtility.DisplayDialog("notice", $"File have been deleted ( Total {fileDelete} )", "ok i know");
        }
#endif

        if (GUILayout.Button("clean as material"))
        {
            var fileDelete = 0;
            var path = folderOuput.PickedPath;
            var materials = GetAtPath<Material>(path);
            foreach (var material in materials)
            {
                int linkReferenceCount = 0;
                var assetDetector = new AssetUsageDetector();
                var result = assetDetector.Run(new AssetUsageDetector.Parameters()
                {
                    objectsToSearch = new Object[] { material },
                    lazySceneSearch = false,
                    searchRefactoring = (searchMatch) =>
                    {
                        linkReferenceCount++;
                    },
                    showDetailedProgressBar = false,
#if ASSET_USAGE_ADDRESSABLES
            addressablesSupport = true,
#endif
                });
                fileDelete += CleanAsMaterial(material, linkReferenceCount) ? 1 : 0;
            }
            EditorUtility.DisplayDialog("notice", $"File have been deleted ( Total {fileDelete} )", "ok i know");
        }

        if (GUILayout.Button("clean as sprite"))
        {
            var fileDelete = 0;
            var path = folderOuput.PickedPath;
            var objects = GetAtPath<Sprite>(path);
            foreach (var obj in objects)
            {
                int linkReferenceCount = 0;
                var assetDetector = new AssetUsageDetector();
                var result = assetDetector.Run(new AssetUsageDetector.Parameters()
                {
                    objectsToSearch = new Object[] { obj },
                    lazySceneSearch = false,
                    searchRefactoring = (searchMatch) =>
                    {
                        linkReferenceCount++;
                    },
                    showDetailedProgressBar = false,
#if ASSET_USAGE_ADDRESSABLES
            addressablesSupport = true,
#endif
                });
                fileDelete += CleanAsNormal(obj, linkReferenceCount) ? 1 : 0;
            }
            EditorUtility.DisplayDialog("notice", $"File have been deleted ( Total {fileDelete} )", "ok i know");
        }

        if (GUILayout.Button("clean as texture"))
        {
            var fileDelete = 0;
            var path = folderOuput.PickedPath;
            var objects = GetAtPath<Texture>(path);
            foreach (var obj in objects)
            {
                int linkReferenceCount = 0;
                var assetDetector = new AssetUsageDetector();
                var result = assetDetector.Run(new AssetUsageDetector.Parameters()
                {
                    objectsToSearch = new Object[] { obj },
                    lazySceneSearch = false,
                    searchRefactoring = (searchMatch) =>
                    {
                        linkReferenceCount++;
                    },
                    showDetailedProgressBar = false,
#if ASSET_USAGE_ADDRESSABLES
            addressablesSupport = true,
#endif
                });
                fileDelete += CleanAsNormal(obj, linkReferenceCount) ? 1 : 0;
            }
            EditorUtility.DisplayDialog("notice", $"File have been deleted ( Total {fileDelete} )", "ok i know");
        }

        if (GUILayout.Button("clean as object"))
        {
            var fileDelete = 0;
            var path = folderOuput.PickedPath;
            var objects = GetAtPath<Object>(path);
            foreach (var obj in objects)
            {
                var linkReferenceCount = 0;
                var assetDetector = new AssetUsageDetector();
                var result = assetDetector.Run(new AssetUsageDetector.Parameters()
                {
                    objectsToSearch = new Object[] { obj },
                    lazySceneSearch = false,
                    searchRefactoring = (searchMatch) =>
                    {
                        linkReferenceCount++;
                    },
                    showDetailedProgressBar = false,
#if ASSET_USAGE_ADDRESSABLES
            addressablesSupport = true,
#endif
                });
                fileDelete += CleanAsNormal(obj, linkReferenceCount) ? 1 : 0;
            }
            EditorUtility.DisplayDialog("notice", $"File have been deleted ( Total {fileDelete} )", "ok i know");
        }
    }

    private bool CleanAsMaterial(Material mat, int linkCount)
    {
        var path = AssetDatabase.GetAssetPath(mat);
        var shaderName = mat.shader.name;
        UnityEngine.Debug.Log($"Link Reference COunt -- {linkCount} ");
        if (linkCount == 0 && !shaderName.Contains(SHADER_GRAPH_STR) && !path.Contains(FBX_FILE) && !path.Contains(META_FILE))
        {
            UnityEngine.Debug.Log($"[FILE-REMOVE] {path}");
            System.IO.File.Delete(path);
            return true;
        }
        return false;
    }

    private bool CleanAsNormal(Object obj, int linkCount)
    {
        var path = AssetDatabase.GetAssetPath(obj);
        if (linkCount == 0 && !path.Contains(META_FILE))
        {
            UnityEngine.Debug.Log($"[FILE-REMOVE] {path}");
            System.IO.File.Delete(path);
            return true;
        }
        return false;
    }

    public static T[] GetAtPath<T>(string path)
    {
        ArrayList al = new ArrayList();
        HashSet<string> hashDirectories = new HashSet<string>();
        var directories = System.IO.Directory.GetDirectories(path).ToList();
        hashDirectories.Add(path);

        while (directories.Count > 0)
        {
            var childDir = new List<string>();
            foreach (var parentDir in directories)
            {
                hashDirectories.Add(parentDir);

                var values = System.IO.Directory.GetDirectories(parentDir).ToList(); 
                childDir.AddRange(values);
                foreach (var directory in values)
                    hashDirectories.Add(directory);
            }
            directories = childDir;
        }

        foreach (var dirPath in hashDirectories)
        {
            string[] fileEntries = System.IO.Directory.GetFiles(dirPath);

            foreach (string fileName in fileEntries)
            {
                int assetPathIndex = fileName.IndexOf("Assets");
                string localPath = fileName.Substring(assetPathIndex);

                Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

                if (t != null)
                    al.Add(t);
            }
        }

        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++)
            result[i] = (T)al[i];

        return result;
    }
}

#endif