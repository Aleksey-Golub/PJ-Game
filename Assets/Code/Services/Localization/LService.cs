using System;
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
            LocalizationService = new LocalizationService();
            Load();
        }

        public static AvailableLanguage CurrentLanguage => _localizationService.CurrentLanguage;

        public static ILocalizationService LocalizationService
        {
            get
            {
                return _localizationService;
            }

            internal set
            {
                if (_localizationService != null)
                    _localizationService.LanguageChanged -= OnLanguageChanged;

                _localizationService = value;
                _localizationService.LanguageChanged += OnLanguageChanged;
            }
        }

        public static IReadOnlyList<AvailableLanguage> AvailableLanguages => _localizationService.AvailableLanguages;

        public static event Action LanguageChanged;

        public static string Localize(string key) => _localizationService.Localize(key);
        public static void LoadPreviousLanguage() => _localizationService.LoadPreviousLanguage();
        public static void LoadNextLanguage() => _localizationService.LoadNextLanguage();
        private static void Load() => _localizationService.Load();
        private static void OnLanguageChanged() => LanguageChanged?.Invoke();
    }
}