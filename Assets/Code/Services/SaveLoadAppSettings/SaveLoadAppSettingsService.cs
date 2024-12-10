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

            string appSettingsJSON = _appSettingsService.Settings.ToJson();

            PlayerPrefs.SetString(APP_SETTINGS_KEY, appSettingsJSON);

#if GAME_PUSH && VK_GAMES
            GamePush.GP_Player.Set(APP_SETTINGS_KEY, appSettingsJSON);
            GamePush.GP_Player.Sync();
#endif
        }

        public AppSettings LoadAppSettings()
        {
#if GAME_PUSH && VK_GAMES
            var gpAppSettingsJson = GamePush.GP_Player.GetString(APP_SETTINGS_KEY);
            if (!string.IsNullOrWhiteSpace(gpAppSettingsJson))
            {
                return gpAppSettingsJson.ToDeserialized<AppSettings>();
            }
#endif

            return PlayerPrefs.GetString(APP_SETTINGS_KEY)?
              .ToDeserialized<AppSettings>();
        }
    }
}