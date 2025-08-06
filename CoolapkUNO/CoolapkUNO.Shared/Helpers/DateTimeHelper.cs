using System;

namespace CoolapkUNO.Helpers
{
    public static class DateTimeHelper
    {
        private enum TimeIntervalType
        {
            YearsAgo,
            MonthsAgo,
            DaysAgo,
            HoursAgo,
            MinutesAgo,
            JustNow,
            Soon,
            MinutesLater,
            HoursLater,
            DaysLater,
            MonthsLater,
            YearsLater
        }

        public static string ConvertUnixTimeStampToReadable(this long time) => ConvertUnixTimeStampToReadable(time, DateTimeOffset.UtcNow);

        public static string ConvertUnixTimeStampToReadable(this long time, DateTimeOffset? baseTime) => ConvertDateTimeOffsetToReadable(time.ConvertUnixTimeStampToDateTimeOffset(), baseTime);

        public static string ConvertDateTimeOffsetToReadable(this DateTimeOffset time) => ConvertDateTimeOffsetToReadable(time, DateTimeOffset.UtcNow);

        public static string ConvertDateTimeOffsetToReadable(this DateTimeOffset time, DateTimeOffset? baseTime)
        {
            object obj;
            TimeIntervalType type;

            if (!baseTime.HasValue)
            {
                type = TimeIntervalType.YearsAgo;
                obj = time.LocalDateTime;
            }
            else
            {
                TimeSpan temp = baseTime.Value.Subtract(time);
                switch (temp)
                {
                    case { Days: > 30 }:
                        type = time.Year == baseTime?.Year
                            ? TimeIntervalType.MonthsAgo
                            : TimeIntervalType.YearsAgo;
                        obj = time.LocalDateTime;
                        break;
                    case { TotalDays: > 0 }:
                        type = temp.Days > 0
                            ? TimeIntervalType.DaysAgo
                            : temp.Hours > 0
                                ? TimeIntervalType.HoursAgo
                                : temp.Minutes > 0
                                    ? TimeIntervalType.MinutesAgo
                                    : TimeIntervalType.JustNow;
                        obj = temp;
                        break;
                    case { Days: > -30 }:
                        type = temp.Days < 0
                            ? TimeIntervalType.DaysLater
                            : temp.Hours < 0
                                ? TimeIntervalType.HoursLater
                                : temp.Minutes < 0
                                    ? TimeIntervalType.MinutesLater
                                    : TimeIntervalType.Soon;
                        obj = temp.Negate();
                        break;
                    default:
                        type = time.Year == baseTime?.Year
                            ? TimeIntervalType.MonthsLater
                            : TimeIntervalType.YearsLater;
                        obj = time.LocalDateTime;
                        break;
                }
            }

            return type switch
            {
                TimeIntervalType.YearsAgo or TimeIntervalType.YearsLater => ((DateTime)obj).ToString("D"),
                TimeIntervalType.MonthsAgo or TimeIntervalType.MonthsLater => ((DateTime)obj).ToString("M"),
                TimeIntervalType.DaysAgo => $"{((TimeSpan)obj).Days}天前",
                TimeIntervalType.HoursAgo => $"{((TimeSpan)obj).Hours}小时前",
                TimeIntervalType.MinutesAgo => $"{((TimeSpan)obj).Minutes}分钟前",
                TimeIntervalType.JustNow => "刚刚",
                TimeIntervalType.Soon => "不久之后",
                TimeIntervalType.MinutesLater => $"{((TimeSpan)obj).Minutes}分钟之后",
                TimeIntervalType.HoursLater => $"{((TimeSpan)obj).Hours}小时之后",
                TimeIntervalType.DaysLater => $"{((TimeSpan)obj).Days}天之后",
                _ => string.Empty,
            };
        }

        public static DateTimeOffset ConvertUnixTimeStampToDateTimeOffset(this long time) =>
            time < 100000_00000
                ? DateTimeOffset.FromUnixTimeSeconds(time)
                : DateTimeOffset.FromUnixTimeMilliseconds(time);

        public static DateTime ConvertDateTimeOffsetToDateTime(this DateTimeOffset dateTime) =>
            dateTime.Offset.Equals(TimeSpan.Zero)
                ? dateTime.UtcDateTime
                : dateTime.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(dateTime.DateTime))
                    ? DateTime.SpecifyKind(dateTime.DateTime, DateTimeKind.Local)
                    : dateTime.DateTime;
    }
}
