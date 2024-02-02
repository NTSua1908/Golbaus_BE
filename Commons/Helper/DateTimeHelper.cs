namespace Golbaus_BE.Commons.Helper
{
	public static class DateTimeHelper
	{
		public static DateTime GetVietnameTime()
		{
			DateTime utcNow = DateTime.UtcNow;
			// Get the Vietnam Standard Time zone
			TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
			// Convert UTC time to Vietnam Standard Time
			return TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
		}

		public static DateTime ConvertVietnameTime(DateTime time)
		{
			TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
			return TimeZoneInfo.ConvertTimeFromUtc(time, vietnamTimeZone);
		}
	}
}
