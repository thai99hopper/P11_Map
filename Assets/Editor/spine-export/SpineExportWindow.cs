using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class SpineExportWindow : EditorWindow
{
    private EditorUIElement_pickFolder folderInput = new EditorUIElement_pickFolder("input");
    private EditorUIElement_pickFolder folderOuput = new EditorUIElement_pickFolder("output");

    public int maxWidth = 2048;
    public int maxHeight = 2048;

    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/spine export")]
    static void Init()
    {
        var window = (SpineExportWindow)GetWindow(typeof(SpineExportWindow));
        window.titleContent = new GUIContent("spine export");
        window.Show();
    }

    private void OnGUI()
    {
        folderInput.Draw();
        folderOuput.Draw();

        EditorGUILayout.BeginVertical();
        maxWidth = int.Parse(EditorGUILayout.TextField(label: "Max width", maxWidth.ToString()));
        maxHeight = int.Parse(EditorGUILayout.TextField(label: "Max height", maxHeight.ToString()));
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Export"))
        {
            if (string.IsNullOrEmpty(GetWorkingDirectory()))
            {
                StaticUtilsEditor.DisplayDialog(
                    "Setup Enviroment Variables for PC before use this function\n" +
                    "Start -> View advanced system settings -> Advanced -> Enviroment Variables\n" +
                    "Key: SPINE\n" +
                    "Value: Directory install spine" +
                    "*** DON'T FORGET RESTART AFTER SET ENVIROMENT ***");
            }
            else
            {
                OnClickExport().Forget();
            }
        }
    }

    private async UniTaskVoid OnClickExport()
    {
        await Export();
        StaticUtilsEditor.DisplayDialog("Export Success");
    }

    private string GetWorkingDirectory()
    {
        var ret = System.Environment.GetEnvironmentVariable("SPINE");

        return ret;
    }

    private string GetSettingFileName()
    {
        var exportSettingFileName = folderInput.PickedPath + @"\" + Guid.NewGuid() + ".json";

        using (StreamWriter ws = new StreamWriter(exportSettingFileName))
        {
            ws.Write(StaticUtils.JsonSerializeToFriendlyText(new TexturePackingSetting(maxWidth, maxHeight)));
        }

        return exportSettingFileName;
    }

    private List<string> GetAllSpineInFolder(bool onlyName = false)
    {
        var ret = new List<string>();

        foreach (var fileName in StaticUtils.GetFilesUnderFolder(folderInput.PickedPath, "spine", true))
        {
            if (onlyName)
            {
                ret.Add(fileName);
            }
            else
            {
                ret.Add($"{folderInput.PickedPath}/{fileName}");
            }
        }

        var lSubFolder = Directory.GetDirectories(folderInput.PickedPath);
        if (lSubFolder != null)
        {
            foreach (var sub in lSubFolder)
            {
                foreach (var fileName in StaticUtils.GetFilesUnderFolder(sub, "spine", true))
                {
                    if (onlyName)
                    {
                        ret.Add(fileName);
                    }
                    else
                    {
                        ret.Add($"{sub}/{fileName}");
                    }
                }
            }
        }

        return ret;
    }

    private async UniTask Export()
    {
        await UniTask.DelayFrame(1);
        var exportSettingFileName = GetSettingFileName();
        var arguments = new List<string>();

        var cmdSpine = "Spine ";

        var spineInput = GetAllSpineInFolder();
        var spineNameOutput = GetAllSpineInFolder(onlyName: true);

        for (int i = 0; i < spineInput.Count; i++)
        {
            cmdSpine += $"-i {spineInput[i]}.spine -o {folderOuput.PickedPath}/{spineNameOutput[i]} -e {exportSettingFileName} ";
        }

        arguments.Add(cmdSpine);

        string batFileName = folderInput.PickedPath + @"\" + Guid.NewGuid() + ".bat";

        using (StreamWriter batFile = new StreamWriter(batFileName))
        {
            foreach (var arg in arguments)
            {
                batFile.WriteLine(arg);
            }
        }

        ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c" + batFileName);
        processInfo.WindowStyle = ProcessWindowStyle.Normal;
        processInfo.UseShellExecute = false;
        processInfo.CreateNoWindow = true;
        processInfo.WorkingDirectory = GetWorkingDirectory();
        processInfo.RedirectStandardError = true;
        processInfo.RedirectStandardOutput = true;

        var process = Process.Start(processInfo);

        //process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
        //    UnityEngine.Debug.Log("output>> " + e.Data);
        //process.BeginOutputReadLine();

        //process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
        //    UnityEngine.Debug.Log("error>> " + e.Data);
        //process.BeginErrorReadLine();

        process.WaitForExit();

        process.Close();
        File.Delete(batFileName);
        File.Delete(exportSettingFileName);
    }
}
