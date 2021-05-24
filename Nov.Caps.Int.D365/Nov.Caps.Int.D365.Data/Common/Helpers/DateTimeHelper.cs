using System;

namespace Nov.Caps.Int.D365.Data.Common.Helpers
{
    public static class DateTimeHelper
    {
        public static string ToTimeID(DateTime dateTime)
        {
            var month = dateTime.Month + 1;
            var yyyy = dateTime.Year.ToString();
            var mm = month.ToString();

            if (month < 10)
            {
                mm = $"0{mm}";
            }

            return $"{yyyy}{mm}00";
        }
    }
}
