
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine;

public static class StaticUtilsEditor
{
	#region other

	public static Vector2 CalculateTextSize(string text, EditorUIComponentType componentType)
	{
		var style = componentType switch
		{
			EditorUIComponentType.label => GUI.skin.label,
			EditorUIComponentType.button => GUI.skin.button,
			_ => null,
		};
		var sz = style.CalcSize(new GUIContent(text));
		return sz;
	}

	//path must be relative, like Assets/..........
	public static void ModifyAsset<T>(string path, UnityAction<T> callback) where T : UnityEngine.Object
	{
		var asset = AssetDatabase.LoadAssetAtPath<T>(path);
		callback?.Invoke(asset);

		AssetDatabase.SaveAssets();

		// Re-import font asset to get the new updated version.
		AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset));

		AssetDatabase.Refresh();
	}

	#endregion

	#region windows special folder

	public static string RandomATempPath(string fileExtension = null)
	{
		var projectPath = StaticUtils.GetProjectPath();
		var randomPath = FileUtil.GetUniqueTempPathInProject();
		var extension = string.IsNullOrEmpty(fileExtension) ? "" : $".{fileExtension}";
		return $"{projectPath}/{randomPath}{extension}";
	}

	public static string GetProgramFilesFolder()
	{
		return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
	}

	#endregion

	#region open file

	//path must be relative, like Assets/..........
	public static void OpenScript(string path, int line)
	{
		var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
		AssetDatabase.OpenAsset(asset, line);
	}

	public static void OpenTextFile(string path)
	{
		var textEditorPath = $"{GetProgramFilesFolder()}/Notepad++/notepad++.exe";
		Process.Start(textEditorPath, path);
	}

	#endregion

	#region open editor dialog

	public static void DisplayDialog(string msg)
	{
		EditorUtility.DisplayDialog("alert", msg, "ok");
	}

	public static string DisplayChooseFolderDialog(string title)
	{
		return EditorUtility.OpenFolderPanel(title, "", "");
	}

	public static string DisplayChooseFileDialog(string title, List<string> lExtension)
	{
		var extensionSB = new StringBuilder("");
		if (lExtension != null)
		{
			for (var i = 0; i < lExtension.Count; i++)
			{
				extensionSB.Append(lExtension[i]);
				if (i < lExtension.Count - 1)
				{
					extensionSB.Append(",");
				}
			}
		}

		return EditorUtility.OpenFilePanel(title, "", extensionSB.ToString());
	}

	public static bool DisplayConfirmDialog(string msg)
	{
		return EditorUtility.DisplayDialog("confirm", msg, "yes", "no");
	}

	#endregion

	#region native app

	public static void OpenFileExplorer(string path)
	{
		Process.Start("explorer.exe", path);
	}

	public static RunBatchScriptOutput RunBatchScript(string command, List<string> args = null, string workingDir = null)
	{
		var psi = new ProcessStartInfo()
		{
			FileName = command,
			UseShellExecute = false,
			RedirectStandardOutput = true,
		};

		if (args != null)
		{
			var sb = new StringBuilder();
			foreach (var arg in args)
			{
				sb.Append($"{arg} ");
			}
			psi.Arguments = sb.ToString();
		}

		if (workingDir != null)
		{
			psi.WorkingDirectory = workingDir;
		}

		var p = Process.Start(psi);
		var output = p.StandardOutput.ReadToEnd();
		p.WaitForExit();

		if (!string.IsNullOrEmpty(output))
		{
			UnityEngine.Debug.Log(output);
		}

		return new RunBatchScriptOutput()
		{
			isSuccess = p.ExitCode == 0,
			output = output,
		};
	}

	public static bool RunShellScript(string scriptPath, List<string> args = null)
	{
		if (!StaticUtils.CheckFileExist(scriptPath, isAbsolutePath: true))
		{
			UnityEngine.Debug.LogError($"file {scriptPath} not found");
			return false;
		}

		var batchScriptArgs = new List<string>() { scriptPath };
		if (args != null)
		{
			batchScriptArgs.AddRange(args);
		}

		var result = RunBatchScript("sh.exe", batchScriptArgs);
		return result.isSuccess;
	}

	#endregion
}