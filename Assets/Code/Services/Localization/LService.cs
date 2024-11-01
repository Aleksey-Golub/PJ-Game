using System.Collections.Generic;

namespace Code.Services
{
    /// <summary>
    /// Localization service Ambient Context
    /// </summary>
    public static class LService
    {
        private static ILocalizationService _localizationService;

        static LService()
        {
            _localizationService = new LocalizationService();
            Load();
        }

        public static ILocalizationService LocalizationService
        {
            get { return _localizationService; }
            internal set { _localizationService = value; }
        }

        public static IReadOnlyList<AvailableLanguage> AvailableLanguages => _localizationService.AvailableLanguages;

        public static string Localize(string key) => _localizationService.Localize(key);
        private static void Load() => _localizationService.Load();
    }
}