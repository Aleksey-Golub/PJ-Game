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

            Debug.Log(progress.ToJson());

            PlayerPrefs.SetString(PROGRESS_KEY, progress.ToJson());
        }

        public GameProgress LoadProgress()
        {
            return PlayerPrefs.GetString(PROGRESS_KEY)?
              .ToDeserialized<GameProgress>();
        }
    }
}