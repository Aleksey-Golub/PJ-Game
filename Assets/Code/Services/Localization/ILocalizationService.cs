using System.Collections.Generic;

namespace Code.Services
{
    public interface ILocalizationService : IService
    {
        IReadOnlyList<AvailableLanguage> AvailableLanguages { get; }

        void Load();
        string Localize(string key);
    }
}