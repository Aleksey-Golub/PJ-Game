using Code.Data;
using UnityEngine;
using System;

namespace Code.Services
{
    public class SaveLoadIAPDataService : ISaveLoadIAPDataService
    {
        public const string IAP_DATA = "IAPData";

        private readonly IIAPService _iapService;

        public SaveLoadIAPDataService (
            IIAPService iapService
            )
        {
            _iapService = iapService;
        }

        public void SaveIAPData()
        {
            PlayerIAPsData purchasedIAPData = _iapService.PlayerIAPsData;

            purchasedIAPData.SaveTime = SaveLoadHelper.GetNowTimeToString();

            string dataJSON = purchasedIAPData.ToJson();

            PlayerPrefs.SetString(IAP_DATA, dataJSON);

#if GAME_PUSH
            if (GamePush.GP_Platform.Type() is not GamePush.Platform.RUSTORE)
            {
                GamePush.GP_Player.Set(IAP_DATA, dataJSON);
                GamePush.GP_Player.Sync();
            }
#endif
        }

        public PlayerIAPsData LoadIAPData()
        {
            string json = PlayerPrefs.GetString(IAP_DATA);
            //Debug.LogError(json);
            PlayerIAPsData prefsData = json?.ToDeserialized<PlayerIAPsData>();

#if GAME_PUSH && !UNITY_EDITOR
            var gpPlatform = GamePush.GP_Platform.Type();
            if (gpPlatform is not GamePush.Platform.RUSTORE)
            {
                var gpDataJson = GamePush.GP_Player.GetString(IAP_DATA);
                if (!string.IsNullOrWhiteSpace(gpDataJson))
                {
                    PlayerIAPsData gpData = gpDataJson.ToDeserialized<PlayerIAPsData>();

                    if (prefsData == null)
                    {
                        return gpData;
                    }
                    else
                    {
                        DateTime prefsTime = SaveLoadHelper.GetTimeFromString(prefsData.SaveTime);
                        DateTime gpTime = SaveLoadHelper.GetTimeFromString(gpData.SaveTime);

                        Logger.Log($"[SaveLoadIAPDataService] prefsTime= {prefsTime}, gpTime= {gpTime}");

                        if (gpPlatform is GamePush.Platform.VK)
                            return DateTime.Compare(prefsTime, gpTime) <= 0 ? gpData : prefsData;
                        else
                            return gpData;
                    }
                }
            }
#endif

            return prefsData;
        }
    }
}