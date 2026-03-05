using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class AssetReplaceTool : EditorWindow
{
    string inputFolder = "";
    string outputFolder = "";

    StringBuilder report = new StringBuilder();

    int totalInput = 0;
    int totalReplace = 0;
    int totalMissing = 0;

    [MenuItem("Tools/Asset Replace Tool")]
    static void Init()
    {
        GetWindow<AssetReplaceTool>("Asset Replace Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("Asset Replace Tool", EditorStyles.boldLabel);

        // Description
        EditorGUILayout.HelpBox(
            "This tool replaces assets in the Output Folder by matching filename and extension.\n\n" +
            "- Input Folder: Source assets (can be outside Unity project)\n" +
            "- Output Folder: Target assets (usually inside Unity Assets folder)\n" +
            "- Files are matched by name + extension\n" +
            "- .meta files are ignored\n" +
            "- A replace report will be generated after execution.",
            MessageType.Info
        );

        GUILayout.Space(10);

        DrawFolderField("Input Folder (External Allowed)", ref inputFolder);

        GUILayout.Space(10);

        DrawFolderField("Output Folder", ref outputFolder);

        GUILayout.Space(15);

        if (GUILayout.Button("Replace Assets", GUILayout.Height(40)))
        {
            ReplaceAssets();
        }
    }

    void DrawFolderField(string label, ref string path)
    {
        GUILayout.Label(label);

        GUILayout.BeginHorizontal();

        EditorGUILayout.TextField(path);

        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            string selected = EditorUtility.OpenFolderPanel("Select Folder", "", "");

            if (!string.IsNullOrEmpty(selected))
                path = selected;
        }

        GUILayout.EndHorizontal();
    }

    void ReplaceAssets()
    {
        if (string.IsNullOrEmpty(inputFolder))
        {
            Debug.LogError("Input folder missing");
            return;
        }

        if (string.IsNullOrEmpty(outputFolder))
        {
            Debug.LogError("Output folder missing");
            return;
        }

        report.Clear();
        totalInput = 0;
        totalReplace = 0;
        totalMissing = 0;

        report.AppendLine("==== ASSET REPLACE REPORT ====");
        report.AppendLine("Input Folder: " + inputFolder);
        report.AppendLine("Output Folder: " + outputFolder);
        report.AppendLine("");

        var inputFiles = Directory.GetFiles(inputFolder, "*.*", SearchOption.AllDirectories);
        var outputFiles = Directory.GetFiles(outputFolder, "*.*", SearchOption.AllDirectories);

        Dictionary<string, List<string>> outputIndex = new Dictionary<string, List<string>>();

        foreach (var file in outputFiles)
        {
            if (file.EndsWith(".meta")) continue;

            string key = Path.GetFileName(file).ToLower();

            if (!outputIndex.ContainsKey(key))
                outputIndex[key] = new List<string>();

            outputIndex[key].Add(file);
        }

        int progress = 0;

        foreach (var inputFile in inputFiles)
        {
            if (inputFile.EndsWith(".meta")) continue;

            totalInput++;
            progress++;

            EditorUtility.DisplayProgressBar(
                "Replacing Assets",
                Path.GetFileName(inputFile),
                (float)progress / inputFiles.Length
            );

            string key = Path.GetFileName(inputFile).ToLower();

            report.AppendLine("INPUT: " + key);

            if (!outputIndex.ContainsKey(key))
            {
                totalMissing++;
                report.AppendLine("  -> NOT FOUND");
                continue;
            }

            foreach (var target in outputIndex[key])
            {
                File.Copy(inputFile, target, true);

                Debug.Log("Replaced: " + target);

                report.AppendLine("  -> REPLACED: " + target);

                totalReplace++;
            }
        }

        EditorUtility.ClearProgressBar();

        AssetDatabase.Refresh();

        report.AppendLine("");
        report.AppendLine("==== SUMMARY ====");
        report.AppendLine("Input Files: " + totalInput);
        report.AppendLine("Replaced Files: " + totalReplace);
        report.AppendLine("Missing Files: " + totalMissing);

        string reportPath = Path.Combine(Application.dataPath, "../ReplaceReport.txt");

        File.WriteAllText(reportPath, report.ToString());

        Debug.Log(report.ToString());
        Debug.Log("Report saved: " + reportPath);

        EditorUtility.DisplayDialog(
            "Replace Completed",
            $"Replaced: {totalReplace}\nMissing: {totalMissing}\n\nReport saved at project root.",
            "OK"
        );
    }
}