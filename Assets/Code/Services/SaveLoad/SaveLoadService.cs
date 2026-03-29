using Code.Data;
using Code.Infrastructure;
using UnityEngine;
using System;

namespace Code.Services
{
    public class SaveLoadService : ISaveLoadService
    {
        public const string PROGRESS_KEY = "Progress";

        private readonly IPersistentProgressService _progressService;
        private readonly IGameFactory _gameFactory;
        private readonly IResourceFactory _resourceFactory;
        private readonly IToolFactory _toolFactory;

        public SaveLoadService(
            IPersistentProgressService progressService,
            IGameFactory gameFactory,
            IResourceFactory resourceFactory,
            IToolFactory toolFactory
            )
        {
            _progressService = progressService;
            _gameFactory = gameFactory;
            _resourceFactory = resourceFactory;
            _toolFactory = toolFactory;
        }

        public void SaveProgress()
        {
            GameProgress progress = _progressService.Progress;

            foreach (ISavedProgressWriter progressWriter in _gameFactory.ProgressWriters)
                progressWriter.WriteToProgress(progress);

            foreach (ISavedProgressWriter resource in _resourceFactory.DroppedResources)
                resource.WriteToProgress(progress);

            foreach (ISavedProgressWriter resource in _toolFactory.DroppedResources)
                resource.WriteToProgress(progress);

            progress.SaveTime = SaveLoadHelper.GetNowTimeToString();

            string progressJSON = progress.ToJson();

#if LOG_SAVING
            Debug.Log(progressJSON);
#endif

#if GAME_PUSH && !UNITY_EDITOR
            var gpPlatform = GamePush.GP_Platform.Type();
            if (gpPlatform is GamePush.Platform.RUSTORE)
            {
                PlayerPrefs.SetString(PROGRESS_KEY, progressJSON);
                Logger.Log($"[SaveLoadService] PROGRESS saved locally");
            }
            else if (gpPlatform is GamePush.Platform.VK)
            {
                PlayerPrefs.SetString(PROGRESS_KEY, progressJSON);
                Logger.Log($"[SaveLoadService] PROGRESS saved locally");

                GamePush.GP_Player.Set(PROGRESS_KEY, progressJSON);
                GamePush.GP_Player.Sync();
                Logger.Log($"[SaveLoadService] PROGRESS saved on GP");
            }
            else
            {
                GamePush.GP_Player.Set(PROGRESS_KEY, progressJSON);
                GamePush.GP_Player.Sync();
                Logger.Log($"[SaveLoadService] PROGRESS saved on GP");
            }

            return;
#endif

            PlayerPrefs.SetString(PROGRESS_KEY, progressJSON);
            Logger.Log($"[SaveLoadService] PROGRESS saved locally general");
        }

        public GameProgress LoadProgress()
        {
            string json;
            GameProgress prefsProgress;

#if GAME_PUSH && !UNITY_EDITOR
            var gpPlatform = GamePush.GP_Platform.Type();
            if (gpPlatform is GamePush.Platform.RUSTORE)
            {
                json = PlayerPrefs.GetString(PROGRESS_KEY);
                //Debug.LogError(json);
                prefsProgress = json?.ToDeserialized<GameProgress>();

                Logger.Log($"[SaveLoadService] Load from prefs");
                return prefsProgress;
            }
            else if (gpPlatform is GamePush.Platform.VK)
            {
                json = PlayerPrefs.GetString(PROGRESS_KEY);
                //Debug.LogError(json);
                prefsProgress = json?.ToDeserialized<GameProgress>();

                var gpProgressJson = GamePush.GP_Player.GetString(PROGRESS_KEY);
                if (!string.IsNullOrWhiteSpace(gpProgressJson))
                {
                    GameProgress gpProgress = gpProgressJson.ToDeserialized<GameProgress>();

                    if (prefsProgress == null)
                    {
                        return gpProgress;
                    }
                    else
                    {
                        DateTime prefsTime = SaveLoadHelper.GetTimeFromString(prefsProgress.SaveTime);
                        DateTime gpTime = SaveLoadHelper.GetTimeFromString(gpProgress.SaveTime);

                        Logger.Log($"[SaveLoadService] prefsTime= {prefsTime}, gpTime= {gpTime}");

                        return DateTime.Compare(prefsTime, gpTime) <= 0 ? gpProgress : prefsProgress;
                    }
                }
            }
            else
            {
                var gpProgressJson = GamePush.GP_Player.GetString(PROGRESS_KEY);
                GameProgress gpProgress = gpProgressJson.ToDeserialized<GameProgress>();

                Logger.Log($"[SaveLoadService] Load from GP");
                return gpProgress;
            }
#endif

            json = PlayerPrefs.GetString(PROGRESS_KEY);
            //Debug.LogError(json);
            prefsProgress = json?.ToDeserialized<GameProgress>();

            Logger.Log($"[SaveLoadService] Load from prefs general");

            return prefsProgress;
        }
    }
}