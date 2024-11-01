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

        public IReadOnlyList<AvailableLanguage> AvailableLanguages => _availableLanguages;

        public string Localize(string key) => _storage.TryGetValue(key, out var value) ? value : key;

        public void Load()
        {
            string twoLetterISOLanguageName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            Logger.Log("CurrentCulture.Name = " + twoLetterISOLanguageName);

            Load(twoLetterISOLanguageName);
        }

        private void Load(string twoLetterISOLanguageName)
        {
            var localizationStorage = Resources.Load<LocalizationStorage>(LOCALIZATIONSTORAGE_PATH);

            CacheAvailableLanguages(localizationStorage);

            var cashingLang = AvailableLanguagesContains(twoLetterISOLanguageName) ? twoLetterISOLanguageName : "en";
            CacheCurrentLang(localizationStorage, cashingLang);
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

        private void CacheCurrentLang(LocalizationStorage localizationStorage, string twoLetterISOLanguageName)
        {
            _storage.Clear();

            var firstRow = localizationStorage.Rows[0];
            int langIndex = firstRow.Values.IndexOf(twoLetterISOLanguageName);

            for (int i = 1; i < localizationStorage.Rows.Count; i++)
            {
                var row = localizationStorage.Rows[i];
                _storage[row.Key] = row.Values[langIndex];
            }
        }
    }
}