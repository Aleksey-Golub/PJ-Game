using Code.Data;
using UnityEngine;

namespace Code.Services
{
    public class SaveLoadAppSettingsService : ISaveLoadAppSettingsService
    {
        public const string APP_SETTINGS_KEY = "AppSettings";

        private readonly IAppSettingsService _appSettingsService;
        private readonly IAudioService _audio;

        public SaveLoadAppSettingsService(IAppSettingsService appSettingsService, IAudioService audio)
        {
            _appSettingsService = appSettingsService;
            _audio = audio;
        }

        public void SaveAppSettings()
        {
            _audio.WriteToAppSettings(_appSettingsService.Settings);
            LService.WriteToAppSettings(_appSettingsService.Settings);

            PlayerPrefs.SetString(APP_SETTINGS_KEY, _appSettingsService.Settings.ToJson());
        }

        public AppSettings LoadAppSettings()
        {
            return PlayerPrefs.GetString(APP_SETTINGS_KEY)?
              .ToDeserialized<AppSettings>();
        }
    }
}