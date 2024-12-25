using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.ExceptionServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static partial class StaticUtils
{
	#region character

	public static bool IsDigitCharacter(char c)
	{
		return c >= '0' && c <= '9';
	}

	public static bool IsLowercaseAlphabetCharacter(char c)
	{
		return c >= 'a' && c <= 'z';
	}

	public static bool IsUppercaseAlphabetCharacter(char c)
	{
		return c >= 'A' && c <= 'Z';
	}

	public static bool IsAlphabetCharacter(char c)
	{
		return IsLowercaseAlphabetCharacter(c) || IsUppercaseAlphabetCharacter(c);
	}

	#endregion

	#region exception

	public static string AggregateExceptionToString(AggregateException aggE)
	{
		var flatE = aggE.Flatten();
		var sb = new StringBuilder();
		for (var i = 1; i <= flatE.InnerExceptions.Count; i++)
		{
			sb.Append($"exception {i}={flatE.InnerExceptions[i - 1].Message}");
			if (i < flatE.InnerExceptions.Count)
			{
				sb.Append(" ");
			}
		}
		return sb.ToString();
	}

	public static void RethrowException(Exception e)
	{
		ExceptionDispatchInfo.Capture(e).Throw();
	}

	#endregion

	#region json

	public static string JsonSerializeToFriendlyText(object obj)
	{
		var settings = new JsonSerializerSettings();
		settings.Formatting = Formatting.Indented;
		settings.Converters.Add(new StringEnumConverter());
		return JsonConvert.SerializeObject(obj, settings);
	}

	public static T CastJsonValue<T>(object jsonValue)
	{
		if (jsonValue is JObject)
		{
			var jo = (JObject)jsonValue;
			return jo.ToObject<T>();
		}
		else if (jsonValue is JArray)
		{
			var ja = (JArray)jsonValue;
			return ja.ToObject<T>();
		}
		else //string, number, boolean
		{
			return (T)jsonValue;
		}
	}

	public static string FormatJson(string json)
	{
		using (var stringReader = new StringReader(json))
		using (var stringWriter = new StringWriter())
		{
			var jsonReader = new JsonTextReader(stringReader);
			var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
			jsonWriter.WriteToken(jsonReader);
			return stringWriter.ToString();
		}
	}

	public static void JsonPopulateAndClear(string json, object target)
	{
		JsonConvert.PopulateObject(json, target, new JsonSerializerSettings()
		{
			ContractResolver = new JsonContractClearArrayBeforePopulate(),
		});
	}

	#endregion

	#region version

	//version convention: [major].[minor].[build]
	// - dev/test phase: [major], [minor] are always zero, just increase [build]
	// - live phase:
	//   + big update: increase [major]
	//   + minor update: increase [minor]
	//   + daily test/dev build: increase [build]

	public static string IncreaseVersion(string verStr, VersionComponent component = VersionComponent.Build)
	{
		var ver = new Version(verStr);
		var build = ver.Build;
		var minor = ver.Minor;
		var major = ver.Major;
		switch (component)
		{
			case VersionComponent.Build:
				build++;
				break;
			case VersionComponent.Minor:
				minor++;
				build = 0;
				break;
			case VersionComponent.Major:
				major++;
				minor = 0;
				build = 0;
				break;
		}
		return new Version(major, minor, build).ToString();
	}

	public static int CompareVersion(string strVer1, string strVer2)
	{
		return new Version(strVer1).CompareTo(new Version(strVer2));
	}

	#endregion

	#region get/post request

	//if you got InvalidOperationException: Insecure connection not allowed,
	//Go to player settings -> other settings -> configuration -> allow downloads over HTTP -> always allowed

	public static async UniTask<HttpGetResult> GetImageHttpRequest(string url)
	{
		var req = UnityWebRequestTexture.GetTexture(url);
		await req.SendWebRequest();

		var result = new HttpGetResult() { url = url };
		var texture = DownloadHandlerTexture.GetContent(req);
		result.resultAsBinary = texture.EncodeToPNG();

		req.Dispose();
		return result;
	}

	public static async UniTask<HttpGetResult> GetHttpRequest(string url, bool returnText)
	{
		var req = UnityWebRequest.Get(url);
		var op = await req.SendWebRequest();

		var result = new HttpGetResult() { url = url };
		if (returnText)
		{
			result.resultAsText = op.downloadHandler.text;
		}
		else
		{
			result.resultAsBinary = op.downloadHandler.data;
		}

		req.Dispose();
		return result;
	}

	public static async UniTask<string> PostHttpRequest(string url, Dictionary<string, string> form)
	{
		var req = UnityWebRequest.Post(url, form);
		try
		{
			var op = await req.SendWebRequest();
			return op.downloadHandler.text;
		}
		catch (UnityWebRequestException e)
		{
			throw new HttpPostException(e);
		}
		finally
		{
			req.Dispose();
		}
	}

	public static async UniTask<string> PostHttpRequest(string url, string json)
	{
		var req = UnityWebRequest.Put(url, json);
		req.SetRequestHeader("Content-Type", "application/json");
		try
		{
			var op = await req.SendWebRequest();
			return op.downloadHandler.text;
		}
		catch (UnityWebRequestException e)
		{
			throw new HttpPostException(e);
		}
		finally
		{
			req.Dispose();
		}
	}

	#endregion

	#region random

	public static string RandomAnUID(int length,
		bool hasNumber = true, bool hasLowercase = true, bool hasUppercase = true)
	{
		var listChars = new List<char>();
		if (hasNumber)
		{
			for (var c = '0'; c <= '9'; c++)
			{
				listChars.Add(c);
			}
		}
		if (hasLowercase)
		{
			for (var c = 'a'; c <= 'z'; c++)
			{
				listChars.Add(c);
			}
		}
		if (hasUppercase)
		{
			for (var c = 'A'; c <= 'Z'; c++)
			{
				listChars.Add(c);
			}
		}

		var sb = new StringBuilder();
		for (var i = 0; i < length; i++)
		{
			var rNumber = UnityEngine.Random.Range(0, listChars.Count);
			sb.Append(listChars[rNumber]);
		}
		return sb.ToString();
	}

	public static byte[] RandomAByteArray(int length)
	{
		var result = new byte[length];
		for (var i = 0; i < length; i++)
		{
			result[i] = (byte)UnityEngine.Random.Range(0, byte.MaxValue + 1);
		}
		return result;
	}

	#endregion

	#region xor encryption

	public static string XorString(string str, string key)
	{
		var sb = new StringBuilder();
		for (int i = 0; i < str.Length; i++)
		{
			sb.Append((char)(str[i] ^ key[i % key.Length]));
		}
		return sb.ToString();
	}

	public static byte[] XorByteArray(byte[] arr, byte[] key)
	{
		var result = new byte[arr.Length];
		for (var i = 0; i < arr.Length; i++)
		{
			result[i] = (byte)(arr[i] ^ key[i % key.Length]);
		}
		return result;
	}

	#endregion

	#region other utils

	public static void CopyToClipboard(string txt)
	{
		GUIUtility.systemCopyBuffer = txt;
	}

	public static void ExitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}

	public static Texture2D CreateColorTexture(Color color)
	{
		var pix = new Color[1];
		pix[0] = color;

		var result = new Texture2D(1, 1);
		result.SetPixels(pix);
		result.Apply();

		return result;
	}

	public static bool IsAppRunning(string appName)
	{
		return Process.GetProcessesByName(appName).Length > 0;
	}

	public static PlatformType GetCurrentPlatform()
	{
#if UNITY_EDITOR
		return PlatformType.Editor;
#elif UNITY_STANDALONE
        return PlatformType.Standalone;
#elif UNITY_ANDROID
        return PlatformType.Android;
#elif UNITY_IOS
        return PlatformType.Ios;
#endif
	}

	//transaction: task A -> delay(time, cancelToken) -> task B
	//if we use UniTask.Delay, when cancel, it will cancel the whole transaction,
	//mean task B won't be executed
	public static async UniTask DelayNonBreak(float secondsDelay, CancellationToken cancellationToken)
	{
		var t = 0f;
		while (t < secondsDelay && !cancellationToken.IsCancellationRequested)
		{
			await UniTask.DelayFrame(1);
			t += Time.deltaTime;
		}
	}

	#endregion

	#region compression 
	public static class CompressionHelper
	{
		public static string Decompress(string compressedData)
		{
			byte[] gzipData = Convert.FromBase64String(compressedData);
			using (var compressedStream = new MemoryStream(gzipData))
			using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
			using (var resultStream = new MemoryStream())
			{
				gzipStream.CopyTo(resultStream);
				return Encoding.UTF8.GetString(resultStream.ToArray());
			}
		}
		public static string Compress(string data)
		{
			byte[] byteArray = Encoding.UTF8.GetBytes(data);
			using (var memoryStream = new MemoryStream())
			{
				using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
				{
					gzipStream.Write(byteArray, 0, byteArray.Length);
				}
				return Convert.ToBase64String(memoryStream.ToArray());
			}
		}


	}
	#endregion
}