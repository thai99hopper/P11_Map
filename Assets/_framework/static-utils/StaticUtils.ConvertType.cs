using System.Globalization;
using System;

public static partial class StaticUtils
{
	#region string to types

	public static bool StringToBool(string s)
	{
		return bool.Parse(s);
	}

	public static int StringToInt(string s)
	{
		return int.Parse(s, CultureInfo.InvariantCulture);
	}

	public static long StringToLong(string s)
	{
		return long.Parse(s, CultureInfo.InvariantCulture);
	}

	public static ulong StringToULong(string s)
	{
		return ulong.Parse(s, CultureInfo.InvariantCulture);
	}

	public static float StringToFloat(string s)
	{
		return float.Parse(s, CultureInfo.InvariantCulture);
	}

	public static double StringToDouble(string s)
	{
		return double.Parse(s, CultureInfo.InvariantCulture);
	}

	public static decimal StringToDecimal(string s)
	{
		return decimal.Parse(s, CultureInfo.InvariantCulture);
	}

	public static DateTime StringToDateTime(string s)
	{
		return DateTime.Parse(s, CultureInfo.InvariantCulture);
	}

	public static T StringToEnum<T>(string s) where T : struct
	{
		if (Enum.TryParse(s, out T val))
		{
			return val;
		}
		else
		{
			return IntToEnum<T>(StringToInt(s));
		}
	}

	public static ulong StringToBytes(string str)
	{
		var i = 0;
		while (str[i] == '.' || IsDigitCharacter(str[i]))
		{
			i++;
		}
		var val = StringToFloat(str.Substring(0, i));

		i = str.Length - 1;
		while (IsAlphabetCharacter(str[i]))
		{
			i--;
		}
		var unit = str.Substring(i + 1);

		return unit.ToLower() switch
		{
			"b" => (ulong)val,
			"kb" => (ulong)(val * 1024),
			"mb" => (ulong)(val * 1024 * 1024),
			"gb" => (ulong)(val * 1024 * 1024 * 1024),
			_ => throw new Exception($"unit {unit} is invalid for bytes")
		};
	}

    #endregion

    #region double to types
    public static string DoubleToString(double s)
    {
        return s.ToString(CultureInfo.InvariantCulture);
    }
    #endregion

    #region char to types
    public static string CharToString(char s)
    {
        return s.ToString(CultureInfo.InvariantCulture);
    }
    #endregion

    #region types to string

    public static string BytesToString(ulong bytes)
	{
		if (bytes < 1024)
		{
			return $"{bytes} B";
		}

		if (bytes < 1024 * 1024)
		{
			var kb = (float)bytes / 1024;
			return $"{kb:0.##} KB";
		}

		if (bytes < 1024 * 1024 * 1024)
		{
			var mb = (float)bytes / 1024 / 1024;
			return $"{mb:0.##} MB";
		}

		var gb = (float)bytes / 1024 / 1024 / 1024;
		return $"{gb:0.##} GB";
	}

	public static string TimespanToString(TimeSpan timeSpan)
	{
		if (timeSpan.Days > 0)
		{
			return $"{timeSpan.Days}d {timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
		}

		if (timeSpan.Hours > 0)
		{
			return $"{timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
		}

		if (timeSpan.Minutes > 0)
		{
			return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
		}

		return $"{timeSpan.Seconds}s";
	}

	#endregion

	#region others

	public static T IntToEnum<T>(int i) where T : struct
	{
		return (T)Enum.ToObject(typeof(T), i);
	}

	#endregion
}