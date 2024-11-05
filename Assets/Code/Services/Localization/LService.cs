using Code.Data;
using System;
using System.Collections.Generic;

namespace Code.Services
{
    /// <summary>
    /// Localization service
    /// </summary>
    public static class LService
    {
        private static ILocalizationService _localizationService;

        public static AvailableLanguage CurrentLanguage => LocalizationService.CurrentLanguage;

        public static ILocalizationService LocalizationService
        {
            get
            {
                if (_localizationService == null)
                {
                    _localizationService = new LocalizationService();
                    _localizationService.LanguageChanged += OnLanguageChanged;
                }
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
        
        public static IReadOnlyList<AvailableLanguage> AvailableLanguages => LocalizationService.AvailableLanguages;

        public static event Action LanguageChanged;

        public static void Load() => LocalizationService.Load();
        public static void WriteToAppSettings(AppSettings settings) => LocalizationService.WriteToAppSettings(settings);
        public static void ReadAppSettings(AppSettings appSettings) => LocalizationService.ReadAppSettings(appSettings);
        public static string Localize(string key) => LocalizationService.Localize(key);
        public static void LoadPreviousLanguage() => LocalizationService.LoadPreviousLanguage();
        public static void LoadNextLanguage() => LocalizationService.LoadNextLanguage();
        private static void OnLanguageChanged() => LanguageChanged?.Invoke();
    }
}