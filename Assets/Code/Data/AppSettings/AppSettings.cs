using System;

namespace Code.Data
{
    [Serializable]
    public class AppSettings
    {
        public LanguageSettings LanguageSettings;
        public AudioSettings AudioSettings;

        public AppSettings()
        {
            LanguageSettings = new LanguageSettings();
            AudioSettings = new AudioSettings();
        }
    }
}