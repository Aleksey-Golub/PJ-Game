using Code.Data;
using System;
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

#if GAME_PUSH && (VK_GAMES || YG)
            GamePush.GP_Player.Set(APP_SETTINGS_KEY, appSettingsJSON);
            GamePush.GP_Player.Sync();
#endif
        }

        public AppSettings LoadAppSettings()
        {
            AppSettings prefsAppSettings = PlayerPrefs.GetString(APP_SETTINGS_KEY)?.ToDeserialized<AppSettings>();

#if GAME_PUSH && (VK_GAMES || YG)
            var gpAppSettingsJson = GamePush.GP_Player.GetString(APP_SETTINGS_KEY);
            if (!string.IsNullOrWhiteSpace(gpAppSettingsJson))
            {
                AppSettings gpAppSettings = gpAppSettingsJson.ToDeserialized<AppSettings>();

                if (prefsAppSettings == null)
                {
                    return gpAppSettings;
                }
                else
                {
                    DateTime prefsTime = SaveLoadHelper.GetTimeFromString(prefsAppSettings.SaveTime);
                    DateTime gpTime = SaveLoadHelper.GetTimeFromString(gpAppSettings.SaveTime);

                    return DateTime.Compare(prefsTime, gpTime) < 0 ? gpAppSettings : prefsAppSettings;
                }
            }
#endif

            return prefsAppSettings;
        }
    }
}