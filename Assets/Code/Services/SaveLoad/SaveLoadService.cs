using Code.Data;
using Code.Infrastructure;
using System;
using UnityEngine;

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

            PlayerPrefs.SetString(PROGRESS_KEY, progressJSON);

#if GAME_PUSH && (VK_GAMES || YG)
            GamePush.GP_Player.Set(PROGRESS_KEY, progressJSON);
            GamePush.GP_Player.Sync();
#endif
        }

        public GameProgress LoadProgress()
        {
            GameProgress prefsProgress = PlayerPrefs.GetString(PROGRESS_KEY)?.ToDeserialized<GameProgress>();

#if GAME_PUSH && (VK_GAMES || YG)
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

                    return DateTime.Compare(prefsTime, gpTime) < 0 ? gpProgress : prefsProgress;
                }
            }
#endif

            return prefsProgress;
        }
    }
}