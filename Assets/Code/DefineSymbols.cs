public static class DefineSymbols
{
    public static readonly string[] Symbols = new string[]
    {
        "ENEBLE_LOGS",
        "ENEBLE_WARNINGS",
        "LOG_SAVING",
        "FAST_DEBUG",
        "FAST_DEBUG_AUTOSAVE",      // set short autosave interval
        "FAST_DEBUG_ADS_INTERVAL",  // set short interval for interstitial ads and Premium and Support screens
        "FAKE_ADS",
        "GAME_PUSH",
        "DEBUG_SHOW_ADS_OBJECTS",   // show Ads objects regardless Ads enabled, usefull for testing in EDITOR
    };
}
