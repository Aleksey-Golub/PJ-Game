using Code.Data;
using System;
using UnityEngine;

namespace Code.Services
{
    public class SaveLoadAnalyticService : ISaveLoadAnalyticService
    {
        public const string ANALYTIC = "Analytic";

        private readonly IAnalyticEventsService _analyticEventsService;

        public SaveLoadAnalyticService(
            IAnalyticEventsService analyticEventsService
            )
        {
            _analyticEventsService = analyticEventsService;
        }

        public void SaveAnalytic()
        {
            AnalyticData analyticEvents = _analyticEventsService.AnalyticData;

            analyticEvents.SaveTime = SaveLoadHelper.GetNowTimeToString();

            string dataJSON = analyticEvents.ToJson();

            PlayerPrefs.SetString(ANALYTIC, dataJSON);

#if GAME_PUSH && (VK_GAMES || YG)
            GamePush.GP_Player.Set(ANALYTIC, dataJSON);
            GamePush.GP_Player.Sync();
#endif
        }

        public AnalyticData LoadAnalytic()
        {
			string json = PlayerPrefs.GetString(ANALYTIC);
            //Debug.LogError(json);
            AnalyticData prefsData = json?.ToDeserialized<AnalyticData>();

#if GAME_PUSH && (VK_GAMES || YG)
            var gpDataJson = GamePush.GP_Player.GetString(ANALYTIC);
            if (!string.IsNullOrWhiteSpace(gpDataJson))
            {
                AnalyticData gpData = gpDataJson.ToDeserialized<AnalyticData>();

                if (prefsData == null)
                {
                    return gpData;
                }
                else
                {
                    DateTime prefsTime = SaveLoadHelper.GetTimeFromString(prefsData.SaveTime);
                    DateTime gpTime = SaveLoadHelper.GetTimeFromString(gpData.SaveTime);

                    Logger.Log($"[SaveLoadAnalyticService] prefsTime= {prefsTime}, gpTime= {gpTime}");

                    return DateTime.Compare(prefsTime, gpTime) < 0 ? gpData : prefsData;
                }
            }
#endif

            return prefsData;
        }
    }
}