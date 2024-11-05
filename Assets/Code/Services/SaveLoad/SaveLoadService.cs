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

        public SaveLoadService(IPersistentProgressService progressService, IGameFactory gameFactory)
        {
            _progressService = progressService;
            _gameFactory = gameFactory;
        }

        public void SaveProgress()
        {
            foreach (ISavedProgressWriter progressWriter in _gameFactory.ProgressWriters)
                progressWriter.WriteToProgress(_progressService.Progress);

            PlayerPrefs.SetString(PROGRESS_KEY, _progressService.Progress.ToJson());
        }

        public GameProgress LoadProgress()
        {
            return PlayerPrefs.GetString(PROGRESS_KEY)?
              .ToDeserialized<GameProgress>();
        }
    }
}