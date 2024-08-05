public static class Logger
{
    public static void Log(string msg)
    {
#if ENEBLE_LOGS
        UnityEngine.Debug.Log(msg);
#endif
    }
}
