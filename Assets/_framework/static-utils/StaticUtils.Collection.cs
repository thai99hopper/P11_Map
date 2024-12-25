using System;
using System.Collections.Generic;
using System.Linq;

public static partial class StaticUtils
{
	#region list

	public static T FindMax<T>(this List<T> l, Func<T, int> aggerateValue)
	{
		var result = l.First();
		int value = aggerateValue(l.First());
		l.ForEach(element =>
		{
			if (value < aggerateValue(element))
				result = element;
		});
		return result;
	}

	public static List<List<T>> Split<T>(this List<T> l, int sz)
	{
		var result = new List<List<T>>();
		foreach (var i in l)
		{
			if (result.Count == 0)
			{
				result.Add(new List<T>());
			}
			var lastList = result[result.Count - 1];
			if (lastList.Count >= sz)
			{
				lastList = new List<T>();
				result.Add(lastList);
			}
			lastList.Add(i);
		}
		return result;
	}

	#endregion

	#region array

	public static int IndexOf<T>(this T[] arr, T item)
	{
		for (var i = 0; i < arr.Length; i++)
		{
			if (arr[i].Equals(item))
			{
				return i;
			}
		}
		return -1;
	}

	public static bool Exists<T>(this T[] arr, Predicate<T> match)
	{
		foreach (var i in arr)
		{
			if (match.Invoke(i))
			{
				return true;
			}
		}
		return false;
	}

	public static T Find<T>(this T[] arr, Predicate<T> match) where T : class
	{
		foreach (var i in arr)
		{
			if (match.Invoke(i))
			{
				return i;
			}
		}
		return null;
	}

	public static T? FindStruct<T>(this T[] arr, Predicate<T> match) where T : struct
	{
		foreach (var i in arr)
		{
			if (match.Invoke(i))
			{
				return i;
			}
		}
		return null;
	}

	#endregion
}