using System;
using System.Globalization;

namespace Code.Services
{
    public static class SaveLoadHelper
    {
        public static DateTime GetTimeFromString(string time)
        {
            return DateTime.Parse(time, CultureInfo.InvariantCulture);
        }

        public static string GetNowTimeToString()
        {
            return DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        }
    }
}