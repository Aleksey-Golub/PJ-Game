using Code.Data;
using Code.Infrastructure;
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

            string progressJSON = progress.ToJson();

#if LOG_SAVING
            Debug.Log(progressJSON);
#endif

            PlayerPrefs.SetString(PROGRESS_KEY, progressJSON);

#if GAME_PUSH && VK_GAMES
            GamePush.GP_Player.Set(PROGRESS_KEY, progressJSON);
            GamePush.GP_Player.Sync();
#endif
        }

        public GameProgress LoadProgress()
        {
#if GAME_PUSH && VK_GAMES
            var gpProgressJson = GamePush.GP_Player.GetString(PROGRESS_KEY);
            if (!string.IsNullOrWhiteSpace(gpProgressJson))
            {
                return gpProgressJson.ToDeserialized<GameProgress>();
            }
#endif

            return PlayerPrefs.GetString(PROGRESS_KEY)?
                .ToDeserialized<GameProgress>();
        }
    }
}