using System;

namespace AMS.Data.Utilities
{
    public class DateTimeHelper
    {
        private readonly string _timeZoneId;

        public DateTimeHelper(string timeZoneId)
        {
            _timeZoneId = timeZoneId;
        }

        public DateTime? ConvertUtcToAppTimeZone(DateTime? utcDateTime)
        {
            if (utcDateTime.HasValue)
            {
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_timeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc((DateTime)utcDateTime, timeZoneInfo);
            }
            else
            {
                return null;
            }
        }

        public DateTime GetCurrentUtc()
        {
            return DateTime.UtcNow;
        }
    }
}
