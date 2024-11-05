using System;
using System.Collections.Generic;

namespace Code.Services
{
    public interface ILocalizationService : IService, ISavedAppSettingsReader, ISavedAppSettingsWriter
    {
        AvailableLanguage CurrentLanguage { get; }
        IReadOnlyList<AvailableLanguage> AvailableLanguages { get; }
        event Action LanguageChanged;

        void Load();
        string Localize(string key);
        void LoadPreviousLanguage();
        void LoadNextLanguage();
    }
}