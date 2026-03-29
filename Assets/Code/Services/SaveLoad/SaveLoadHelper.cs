using System;
using System.Globalization;

namespace Code.Services
{
    public static class SaveLoadHelper
    {
        public static DateTime GetTimeFromString(string time)
        {
            try
            {
                return DateTime.Parse(time, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                string timeValue = time == null ? "null" : time == string.Empty ? "emptyString" : "unknown";
                string msg = 
                    $"[SaveLoadHelper] GetTimeFromString() can not parse time='{time}', timeValue='{timeValue}'. Return new DateTime(). \n" +
                    $"exception: {e}.";

                Logger.LogError(msg);
#if GAME_PUSH
                GamePush.GP_Player.Set(Constants.EXCEPTION_DATETIME_PARSE_KEY, msg);
                GamePush.GP_Player.Sync();
#endif
                return new DateTime();
            }
        }

        public static string GetNowTimeToString()
        {
            return DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        }
    }
}