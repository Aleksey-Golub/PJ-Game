using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Code.Services
{
    public class LocalizationService : ILocalizationService
    {
        private const string LOCALIZATIONSTORAGE_PATH = "Localization/LocalizationStorage";

        private readonly List<AvailableLanguage> _availableLanguages = new();
        private readonly Dictionary<string, string> _storage = new();

        public AvailableLanguage CurrentLanguage { get; private set; }
        public IReadOnlyList<AvailableLanguage> AvailableLanguages => _availableLanguages;

        public event Action LanguageChanged;

        public string Localize(string key) => _storage.TryGetValue(key, out var value) ? value : key;

        public void LoadPreviousLanguage()
        {
            Load(_availableLanguages.PreviousCircular(CurrentLanguage).TwoLetterISOLanguageName);
        }

        public void LoadNextLanguage()
        {
            Load(_availableLanguages.NextCircular(CurrentLanguage).TwoLetterISOLanguageName);
        }

        public void Load()
        {
            string systemTwoLetterISOLanguageName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            Logger.Log("CurrentCulture.Name = " + systemTwoLetterISOLanguageName);

            var localizationStorage = Resources.Load<LocalizationStorage>(LOCALIZATIONSTORAGE_PATH);
            CacheAvailableLanguages(localizationStorage);

            string loadingTwoLetterISOLanguageName = AvailableLanguagesContains(systemTwoLetterISOLanguageName) ? systemTwoLetterISOLanguageName : "en";
            Load(loadingTwoLetterISOLanguageName);
        }

        private void Load(string twoLetterISOLanguageName)
        {
            var localizationStorage = Resources.Load<LocalizationStorage>(LOCALIZATIONSTORAGE_PATH);

            CurrentLanguage = _availableLanguages.First(l => l.TwoLetterISOLanguageName == twoLetterISOLanguageName);
            CacheCurrentLang(localizationStorage, CurrentLanguage);

            LanguageChanged?.Invoke();
        }

        private bool AvailableLanguagesContains(string twoLetterISOLanguageName)
        {
            return _availableLanguages.FirstOrDefault(x => x.TwoLetterISOLanguageName == twoLetterISOLanguageName) != null;
        }

        private void CacheAvailableLanguages(LocalizationStorage localizationStorage)
        {
            var firstRow = localizationStorage.Rows[0];
            var secondRow = localizationStorage.Rows[1];
            int langsCount = firstRow.Values.Count;

            for (int i = 0; i < langsCount; i++)
            {
                _availableLanguages.Add(new AvailableLanguage(firstRow.Values[i], secondRow.Values[i]));
            }
        }

        private void CacheCurrentLang(LocalizationStorage localizationStorage, AvailableLanguage language)
        {
            _storage.Clear();

            var firstRow = localizationStorage.Rows[0];
            int langIndex = firstRow.Values.IndexOf(language.TwoLetterISOLanguageName);

            for (int i = 1; i < localizationStorage.Rows.Count; i++)
            {
                var row = localizationStorage.Rows[i];
                _storage[row.Key] = row.Values[langIndex];
            }
        }
    }
}