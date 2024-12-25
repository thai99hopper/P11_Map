using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public static partial class StaticUtils
{
	#region get paths

	private static string GetDataPath()
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		return Application.dataPath;
#else
		return Application.persistentDataPath;
#endif
	}

	public static string GetAbsolutePath(string path, bool isAbsolutePath)
	{
		if (isAbsolutePath)
		{
			return path;
		}
		else
		{
			return $"{GetDataPath()}/{path}";
		}
	}

	public static string GetProjectPath()
	{
		return GetFolderParent(Application.dataPath);
	}

	public static string GetStreamingFullPath(string path)
	{
		return $"{Application.streamingAssetsPath}/{path}";
	}

	#endregion

	#region read from resource folder

	public static string GetResourceFileText(string path)
	{
		return Resources.Load<TextAsset>(path).text;
	}

	#endregion

	#region read from memory

	public static void ReadFromTextMemory(string txt, UnityAction<StreamReader> callback)
	{
		var stream = new MemoryStream();
		var writer = new StreamWriter(stream);
		writer.Write(txt);
		writer.Flush();
		stream.Position = 0;

		var reader = new StreamReader(stream);
		callback?.Invoke(reader);
		writer.Close();
		reader.Close();
		stream.Close();
	}

	public static void ReadFromBinaryMemory(byte[] bytes, UnityAction<BinaryReader> callback)
	{
		var stream = new MemoryStream(bytes);
		var reader = new BinaryReader(stream);
		callback?.Invoke(reader);
		reader.Close();
		stream.Close();
	}

	#endregion

	#region read from streaming assets

	public static async UniTask<string> GetStreamingFileText(string path)
	{
		var fullPath = GetStreamingFullPath(path);
#if UNITY_ANDROID && !UNITY_EDITOR
		var result = await GetHttpRequest(fullPath, returnText: true);
		return result.resultAsText;
#else
		await UniTask.CompletedTask;
		return ReadTextFile(fullPath, isAbsolutePath: true);
#endif
	}

	public static async UniTask<byte[]> GetStreamingFileBinary(string path)
	{
		var fullPath = GetStreamingFullPath(path);
#if UNITY_ANDROID && !UNITY_EDITOR
		var result = await GetHttpRequest(fullPath, returnText: false);
		return result.resultAsBinary;
#else
		await UniTask.CompletedTask;
		return ReadBinaryFile(fullPath, isAbsolutePath: true);
#endif
	}

	#endregion

	#region read file

	public static string ReadTextFile(string path, bool isAbsolutePath = false)
	{
		path = GetAbsolutePath(path, isAbsolutePath);
		var reader = new StreamReader(path);
		var fileContent = reader.ReadToEnd();
		reader.Close();
		return fileContent;
	}

	public static void ReadTextFile(string path, UnityAction<Stream> callback, bool isAbsolutePath = false)
	{
		path = GetAbsolutePath(path, isAbsolutePath);
		var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
		callback?.Invoke(stream);
		stream.Close();
	}

	public static void ReadTextFile(string path, UnityAction<StreamReader> callback, bool isAbsolutePath = false)
	{
		path = GetAbsolutePath(path, isAbsolutePath);
		var reader = new StreamReader(path);
		callback?.Invoke(reader);
		reader.Close();
	}

	public static void ReadTextFileIntoLines(string path, UnityAction<string, int> lineCallback, bool isAbsolutePath = false)
	{
		ReadTextFile(path, (StreamReader streamReader) =>
		{
			var lineNumber = 0;
			while (!streamReader.EndOfStream)
			{
				lineNumber++;
				var line = streamReader.ReadLine();
				lineCallback?.Invoke(line, lineNumber);
			}
		}, isAbsolutePath);
	}

	public static byte[] ReadBinaryFile(string path, bool isAbsolutePath = false)
	{
		path = GetAbsolutePath(path, isAbsolutePath);
		var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
		var buffer = new byte[fileStream.Length];
		fileStream.Read(buffer, 0, buffer.Length);
		fileStream.Close();
		return buffer;
	}

	public static void ReadBinaryFile(string path, UnityAction<BinaryReader> readCallback, bool isAbsolutePath = false)
	{
		path = GetAbsolutePath(path, isAbsolutePath);
		var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
		var reader = new BinaryReader(fileStream);
		readCallback?.Invoke(reader);
		reader.Close();
		fileStream.Close();
	}

	#endregion

	#region write file

	public static void WriteTextFile(string path, string fileContent, bool isAbsolutePath = false)
	{
		path = GetAbsolutePath(path, isAbsolutePath);

		CreateFolder(GetFolderParent(path), isAbsolutePath: true);

		var writer = new StreamWriter(path, append: false);
		writer.Write(fileContent);
		writer.Close();
	}

	public static void WriteTextFile(string path, UnityAction<StreamWriter> callback, bool isAbsolutePath = false)
	{
		path = GetAbsolutePath(path, isAbsolutePath);

		CreateFolder(GetFolderParent(path), isAbsolutePath: true);

		var writer = new StreamWriter(path, append: false);
		callback?.Invoke(writer);
		writer.Close();
	}

	public static void WriteBinaryFile(string path, UnityAction<BinaryWriter> writeCallback, bool isAbsolutePath = false)
	{
		path = GetAbsolutePath(path, isAbsolutePath);

		CreateFolder(GetFolderParent(path), isAbsolutePath: true);

		var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
		var writer = new BinaryWriter(fileStream);
		writeCallback?.Invoke(writer);
		writer.Close();
		fileStream.Close();
	}

	public static void WriteBinaryFile(string path, Stream inputStream, bool isAbsolutePath = false)
	{
		path = GetAbsolutePath(path, isAbsolutePath);

		CreateFolder(GetFolderParent(path), isAbsolutePath: true);

		var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
		if (inputStream.CanSeek)
		{
			inputStream.Seek(0, SeekOrigin.Begin);
		}
		inputStream.CopyTo(fileStream);
		fileStream.Close();
	}

	#endregion

	#region list

	public static string GetSubFolder(string path, bool isAbsolutePath = false)
	{
		path = GetAbsolutePath(path, isAbsolutePath);
		var lSubFolders = Directory.GetDirectories(path);
		if (lSubFolders == null || lSubFolders.Length == 0)
		{
			return null;
		}
		else
		{
			return GetFolderName(lSubFolders[0]);
		}
	}

	public static List<string> GetFilesUnderFolder(string path, string extension,
		bool isAbsolutePath = false)
	{
		path = GetAbsolutePath(path, isAbsolutePath);
		var arr = Directory.GetFiles(path, $"*.{extension}");
		var result = new List<string>();
		foreach (var i in arr)
		{
			result.Add(Path.GetFileNameWithoutExtension(i));
		}
		return result;
	}

	public static List<string> GetFilesUnderFolderAndSub(string path, string filename,
		bool isAbsolutePath = false)
	{
		path = GetAbsolutePath(path, isAbsolutePath);
		var arr = Directory.GetFiles(path, filename, SearchOption.AllDirectories);
		return new List<string>(arr);
	}

	#endregion

	#region path processing

	public static string GetFolderName(string path)
	{
		var info = new DirectoryInfo(path);
		return info.Name;
	}

	public static string GetFolderParent(string path)
	{
		var info = new DirectoryInfo(path);
		return info.Parent.ToString();
	}

	public static string ToFriendlyPath(string path)
	{
		var info = new FileInfo(path);
		return info.FullName;
	}

	#endregion

	#region manipulate file

	public static bool CheckFileExist(string path, bool isAbsolutePath = false)
	{
		path = GetAbsolutePath(path, isAbsolutePath);
		return File.Exists(path);
	}

	public static void CopyFile(string sourceFile, string targetFolder, bool isAbsolutePath = false)
	{
		sourceFile = GetAbsolutePath(sourceFile, isAbsolutePath);
		targetFolder = GetAbsolutePath(targetFolder, isAbsolutePath);

		var filename = Path.GetFileName(sourceFile);
		var targetFile = $"{targetFolder}/{filename}";

		if (!CheckFolderExist(targetFolder, isAbsolutePath: true))
		{
			CreateFolder(targetFolder, isAbsolutePath: true);
		}
		File.Copy(sourceFile, targetFile, overwrite: true);
	}

	public static void RenameFile(string oldPath, string newName, bool isAbsolutePath = false)
	{
		oldPath = GetAbsolutePath(oldPath, isAbsolutePath);
		var newPath = $"{GetFolderParent(oldPath)}/{newName}";
		File.Move(oldPath, newPath);
	}

	public static void DeleteFile(string path, bool isAbsolutePath = false)
	{
		if (CheckFileExist(path, isAbsolutePath))
		{
			path = GetAbsolutePath(path, isAbsolutePath);
			File.Delete(path);
		}
	}

	#endregion

	#region manipulate folder

	public static bool CheckFolderExist(string path, bool isAbsolutePath = false)
	{
		path = GetAbsolutePath(path, isAbsolutePath);
		return Directory.Exists(path);
	}

	public static void CreateFolder(string path, bool isAbsolutePath = false)
	{
		path = GetAbsolutePath(path, isAbsolutePath);
		if (!CheckFolderExist(path, isAbsolutePath: true))
		{
			Directory.CreateDirectory(path);
		}
	}

	public static void DeleteFolder(string path, bool isAbsolutePath = false)
	{
		path = GetAbsolutePath(path, isAbsolutePath);
		if (CheckFolderExist(path, isAbsolutePath: true))
		{
			Directory.Delete(path, recursive: true);
		}
	}

	public static void CopyFolder(string sourcePath, string targetPath, bool isAbsolutePath = false)
	{
		sourcePath = GetAbsolutePath(sourcePath, isAbsolutePath);
		targetPath = GetAbsolutePath(targetPath, isAbsolutePath);

		if (!CheckFolderExist(targetPath, isAbsolutePath: true))
		{
			CreateFolder(targetPath, isAbsolutePath: true);
		}

		foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
		{
			Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
		}

		foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
		{
			File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
		}
	}

	#endregion
}