public static class Logger
{
    public static void Log(string msg)
    {
#if ENEBLE_LOGS || UNITY_EDITOR
        UnityEngine.Debug.Log(msg);
#endif
    }

    public static void LogWarning(string msg)
    {
#if ENEBLE_WARNINGS || UNITY_EDITOR
        UnityEngine.Debug.LogWarning(msg);
#endif
    }

    public static void LogError(string msg)
    {
        UnityEngine.Debug.LogError(msg);
    }
}
