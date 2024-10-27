using System.Collections.Generic;
using Code.Services;
using Code.UI;
using Code.UI.Services;
using UnityEngine;

namespace Code.Infrastructure
{
    public class GameFactory : IGameFactory
    {
        public List<ISavedProgressReader> ProgressReaders { get; } = new List<ISavedProgressReader>();
        public List<ISavedProgress> ProgressWriters { get; } = new List<ISavedProgress>();

        private readonly IAssetProvider _assets;
        private readonly IConfigsService _configs;
        private readonly IPersistentProgressService _progressService;
        private readonly IUIMediator _uiMediator;
        private readonly IAudioService _audio;
        private readonly IInputService _input;
        private readonly IPopupFactory _popupFactory;
        private readonly ITransitionalResourceFactory _transitionalResourceFactory;
        private GameObject _heroGameObject;

        public GameFactory(
            IAssetProvider assets, 
            IConfigsService configs, 
            IPersistentProgressService persistentProgressService, 
            IUIMediator uiMediator,
            IAudioService audio,
            IInputService input,
            IPopupFactory popupFactory,
            ITransitionalResourceFactory transitionalResourceFactory
            )
        {
            _assets = assets;
            _configs = configs;
            _progressService = persistentProgressService;
            _uiMediator = uiMediator;
            _audio = audio;
            _input = input;
            _popupFactory = popupFactory;
            _transitionalResourceFactory = transitionalResourceFactory;
        }

        public GameObject CreateHero(GameObject at)
        {
            _heroGameObject = InstantiateRegistered(AssetPath.HERO_PATH, at.transform.position);
            _heroGameObject.GetComponent<Player>().Construct(_input, _uiMediator.GetPlayerInventoryView(), _configs, _popupFactory, _transitionalResourceFactory, _progressService);

            return _heroGameObject;
        }

        public Hud CreateHud()
        {
            Hud hud = InstantiateRegistered(AssetPath.HUD_PATH).GetComponent<Hud>();

            hud.PlayerInventoryView.Construct(_configs);

            foreach (OpenWindowButton openWindowButton in hud.GetComponentsInChildren<OpenWindowButton>())
                openWindowButton.Construct(_uiMediator, _audio);

            return hud;
        }

        public void Cleanup()
        {
            ProgressReaders.Clear();
            ProgressWriters.Clear();
        }

        private GameObject InstantiateRegistered(string prefabPath, Vector3 at)
        {
            GameObject gameObject = _assets.Instantiate(path: prefabPath, at: at);
            RegisterProgressWatchers(gameObject);

            return gameObject;
        }

        private GameObject InstantiateRegistered(string prefabPath)
        {
            GameObject gameObject = _assets.Instantiate(path: prefabPath);
            RegisterProgressWatchers(gameObject);

            return gameObject;
        }

        private void RegisterProgressWatchers(GameObject gameObject)
        {
            foreach (ISavedProgressReader progressReader in gameObject.GetComponentsInChildren<ISavedProgressReader>())
                Register(progressReader);
        }

        private void Register(ISavedProgressReader progressReader)
        {
            if (progressReader is ISavedProgress progressWriter)
                ProgressWriters.Add(progressWriter);

            ProgressReaders.Add(progressReader);
        }
    }
}