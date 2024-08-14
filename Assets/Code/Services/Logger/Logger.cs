public static class Logger
{
    public static void Log(string msg)
    {
#if ENEBLE_LOGS
        UnityEngine.Debug.Log(msg);
#endif
    }

    public static void LogWarning(string msg)
    {
#if ENEBLE_LOGS
        UnityEngine.Debug.LogWarning(msg);
#endif
    }

    public static void LogError(string msg)
    {
#if ENEBLE_LOGS
        UnityEngine.Debug.LogError(msg);
#endif
    }
}
