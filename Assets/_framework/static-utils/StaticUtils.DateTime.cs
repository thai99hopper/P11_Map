using System;

public static partial class StaticUtils
{
	#region convert unix epoch <-> System.DateTime

	private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	public static long DateTimeToUnixEpoch(DateTime dateTime)
	{
		return (long)(dateTime - epoch).TotalSeconds;
	}

	public static DateTime UnixEpochToDateTime(long unixEpoch)
	{
		return epoch.AddSeconds(unixEpoch);
	}

	#endregion

	#region calculate next day/week/month

	public static DateTime NextMinute(DateTime dateTime)
	{
		return dateTime.AddMinutes(1);
	}

	public static DateTime NextDay(DateTime dateTime)
	{
		return BeginToday(dateTime).AddDays(1);
	}

	public static DateTime NextWeek(DateTime dateTime)
	{
		var daysUntilNextWeek = dateTime.DayOfWeek switch
		{
			DayOfWeek.Sunday => 1,
			DayOfWeek.Monday => 7,
			DayOfWeek.Tuesday => 6,
			DayOfWeek.Wednesday => 5,
			DayOfWeek.Thursday => 4,
			DayOfWeek.Friday => 3,
			DayOfWeek.Saturday => 2,
			_ => -1,
		};
		return BeginToday(dateTime).AddDays(daysUntilNextWeek);
	}

	public static DateTime NextMonth(DateTime dateTime)
	{
		var year = dateTime.Year;
		var month = dateTime.Month;
		var day = 1;
		var hour = 0;
		var minute = 0;
		var second = 0;
		return new DateTime(year, month, day, hour, minute, second, dateTime.Kind).AddMonths(1);
	}

	// dateTime = 6:00AM 1/1/2023
	// hourInDay = 8:00AM =============> 8:00AM 1/1/2023
	// hourInDay = 5:00AM =============> 5:00AM 2/1/2013
	public static DateTime NextTime(DateTime dateTime, string hourInDay)
	{
		var l = hourInDay.Split(':');
		var hour = StringToInt(l[0]);
		var minute = StringToInt(l[1]);

		var d = BeginToday(dateTime).AddHours(hour).AddMinutes(minute);
		if (dateTime < d)
		{
			return d;
		}
		else
		{
			return d.AddDays(1);
		}
	}

	#endregion

	#region other utils

	public static DateTime BeginToday(DateTime dateTime)
	{
		var year = dateTime.Year;
		var month = dateTime.Month;
		var day = dateTime.Day;
		var hour = 0;
		var minute = 0;
		var second = 0;
		return new DateTime(year, month, day, hour, minute, second, dateTime.Kind);
	}

	#endregion
}