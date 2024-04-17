namespace Tgs.GameInterface.CrPati.Core.Extensions;

public static class DateTimeExtensions
{
	private static readonly TimeZoneInfo _TimeZone;

	static DateTimeExtensions()
	{
		_TimeZone = TimeZoneInfo.CreateCustomTimeZone(id: "Taipei Standard Time", baseUtcOffset: TimeSpan.FromHours(+8), "Taipei Standard Time", "Taipei Standard Time");
	}

	public static string DatetimeConvertToString(this DateTimeOffset date)
	{
		return TimeZoneInfo.ConvertTime(date.ToUniversalTime(), _TimeZone).ToString("yyyy-MM-dd HH:mm:ss");
	}

	public static DateTimeOffset StringConvertDateTimeOffset(this string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return DateTime.UnixEpoch;
		}

		var date = DateTime.Parse(str);

		return new DateTimeOffset(date, _TimeZone.GetUtcOffset(date));
	}
}
