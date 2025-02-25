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
        public List<ISavedProgressWriter> ProgressWriters { get; } = new List<ISavedProgressWriter>();

        private readonly IAssetProvider _assets;
        private readonly IConfigsService _configs;
        private readonly IPersistentProgressService _progressService;
        private readonly IUIMediator _uiMediator;
        private readonly IAudioService _audio;
        private readonly IInputService _input;
        private readonly IPopupFactory _popupFactory;
        private readonly IResourceFactory _resourceFactory;
        private readonly IToolFactory _toolFactory;
        private readonly IEffectFactory _effectFactory;
        private readonly IDropCountCalculatorService _dropCountCalculatorService;
        private readonly IAdsService _adsService;
        private readonly ITransitionalResourceFactory _transitionalResourceFactory;
        private readonly CreatedByIdGameObjectsConstructor _createdByIdGameObjectsConstructor;
        private GameObject _heroGameObject;

        public GameFactory(
            IAssetProvider assets,
            IConfigsService configs,
            IPersistentProgressService persistentProgressService,
            IUIMediator uiMediator,
            IAudioService audio,
            IInputService input,
            IPopupFactory popupFactory,
            ITransitionalResourceFactory transitionalResourceFactory,
            IResourceFactory resourceFactory,
            IToolFactory toolFactory,
            IEffectFactory effectFactory,
            IDropCountCalculatorService dropCountCalculatorService,
            IAdsService adsService
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
            _resourceFactory = resourceFactory;
            _toolFactory = toolFactory;
            _effectFactory = effectFactory;
            _dropCountCalculatorService = dropCountCalculatorService;
            _adsService = adsService;

            _createdByIdGameObjectsConstructor = new(this);
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

        public ResourceSource CreateResourceSource(ResourceSourceType type, Vector3 at, bool registerProgressWatchers = true)
        {
            ResourceSourceMatcher rSourceMatcher = _configs.GetMatcherFor(type);
            ResourceSource resourceSource = InstantiateRegistered(rSourceMatcher.Template, at, registerProgressWatchers);

            resourceSource.Construct(_resourceFactory, _dropCountCalculatorService, _audio, _effectFactory);

            return resourceSource;
        }

        public ResourceStorage CreateResourceStorage(ResourceStorageType type, Vector3 at)
        {
            ResourceStorageMatcher rStorageMatcher = _configs.GetMatcherFor(type);
            ResourceStorage resourceStorage = InstantiateRegistered(rStorageMatcher.Template, at);

            resourceStorage.Construct(_resourceFactory, _progressService, _audio, _effectFactory);

            return resourceStorage;
        }

        public SimpleObjectBase CreateSimpleObject(SimpleObjectType type, Vector3 at)
        {
            SimpleObjectMatcher simpleObjectMatcher = _configs.GetMatcherFor(type);
            SimpleObjectBase simpleObject = InstantiateRegistered(simpleObjectMatcher.Template, at);

            switch (type)
            {
                case SimpleObjectType.SellBoard:
                    (simpleObject as SellBoard).Construct(_uiMediator, _configs);
                    break;
                case SimpleObjectType.UpgradeBoard:
                    (simpleObject as UpgradeBoard).Construct(_uiMediator, _configs, _progressService);
                    break;
                case SimpleObjectType.Prize_First:
                    (simpleObject as FinalPrize).Construct(_audio);
                    break;
                case SimpleObjectType.Boots:
                    (simpleObject as BootsAdsObject).Construct(_adsService, _audio);
                    break;
                case SimpleObjectType.AdsResourceBox:
                    (simpleObject as AdsResourceBox).Construct(_adsService, _resourceFactory, _audio);
                    break;
                case SimpleObjectType.TutorialOnly:
                case SimpleObjectType.None:
                default:
                    throw new System.NotSupportedException($"[GameFactory] CreateSimpleObject() not supported for {type}");
            }

            return simpleObject;
        }

        public Workbench CreateWorkbench(Vector3 at)
        {
            Workbench workbench = InstantiateRegistered(AssetPath.WORKBENCH_BASE_PATH, at).GetComponent<Workbench>();
            workbench.Construct(_resourceFactory, _toolFactory, _audio, _effectFactory);

            return workbench;
        }

        public Chunk CreateChunk(Vector3 at)
        {
            Chunk chunk = InstantiateRegistered(AssetPath.CHUNK_BASE_PATH, at).GetComponent<Chunk>();
            chunk.Construct(_audio, _effectFactory, this);

            return chunk;
        }

        public Workshop CreateWorkshop(WorkshopType type, Vector3 at)
        {
            string assetPath;
            switch (type)
            {
                case WorkshopType.WorkshopBase:
                    assetPath = AssetPath.WORKSHOP_BASE_PATH;
                    break;
                case WorkshopType.DryFruitBush:
                    assetPath = AssetPath.DRYFRUITBUSH_PATH;
                    break;
                case WorkshopType.None:
                default:
                    throw new System.NotImplementedException($"[GameFactory] CreateWorkshop(). Not implemented for {type}");
            }

            Workshop workshop = InstantiateRegistered(assetPath, at).GetComponent<Workshop>();
            workshop.Construct(_audio, _effectFactory, this);

            return workshop;
        }

        public Converter CreateConverter(ConverterType type, Vector3 at)
        {
            string assetPath;
            switch (type)
            {
                case ConverterType.CowConverter:
                    assetPath = AssetPath.COW_PATH;
                    break;
                case ConverterType.PigConverter:
                    assetPath = AssetPath.PIG_PATH;
                    break;
                case ConverterType.FurnaceConverter:
                case ConverterType.None:
                default:
                    throw new System.NotImplementedException($"[GameFactory] CreateWorkshop(). Not implemented for {type}");
            }

            Converter converter = InstantiateRegistered(assetPath, at).GetComponent<Converter>();
            converter.Construct(_resourceFactory, _progressService, _audio, _effectFactory);

            return converter;
        }

        public Dungeon CreateDungeon(string gameObjectId, Vector3 at)
        {
            GameObjectMatcher gameObjectMatcher = _configs.GetMatcherFor(gameObjectId);
            Dungeon dungeon = InstantiateRegistered(gameObjectMatcher.Template, at).GetComponent<Dungeon>();

            dungeon.Construct(this, _audio, _effectFactory, _progressService);

            return dungeon;
        }

        public Tutorial CreateTutorial(string sceneName)
        {
            TutorialMatcher tutorialMatcher = _configs.GetMatcherForTutorial(sceneName);
            Tutorial tutorial = InstantiateRegistered(tutorialMatcher.Template).GetComponent<Tutorial>();

            tutorial.Construct(this, _progressService, _heroGameObject.GetComponent<Player>());

            return tutorial;
        }

        public GameObject GetGameObject(string gameObjectId, Vector3 at, bool registerProgressWatchers = true)
        {
            GameObjectMatcher gameObjectMatcher = _configs.GetMatcherFor(gameObjectId);
            GameObject go = InstantiateRegistered(gameObjectMatcher.Template, at, registerProgressWatchers);

            go.GetComponent<ICreatedByIdGameObject>().Accept(_createdByIdGameObjectsConstructor);

            return go;
        }

        public void Recycle(GameObject gameObject)
        {
            UnRegisterProgressWatchers(gameObject);

            Object.Destroy(gameObject);
        }

        public void Cleanup()
        {
            ProgressReaders.Clear();
            ProgressWriters.Clear();
        }

        public void RegisterProgressWatchersExternal(GameObject gameObject) => RegisterProgressWatchers(gameObject);

        private void RegisterProgressWatchers(GameObject gameObject)
        {
            foreach (ISavedProgressReader progressReader in gameObject.GetComponentsInChildren<ISavedProgressReader>())
                ProgressReaders.Add(progressReader);

            foreach (ISavedProgressWriter progressWriter in gameObject.GetComponentsInChildren<ISavedProgressWriter>())
                ProgressWriters.Add(progressWriter);
        }

        private void UnRegisterProgressWatchers(GameObject gameObject)
        {
            foreach (ISavedProgressReader progressReader in gameObject.GetComponentsInChildren<ISavedProgressReader>())
                ProgressReaders.Remove(progressReader);

            foreach (ISavedProgressWriter progressWriter in gameObject.GetComponentsInChildren<ISavedProgressWriter>())
                ProgressWriters.Remove(progressWriter);
        }

        private GameObject InstantiateRegistered(string prefabPath, bool registerProgressWatchers = true)
        {
            GameObject gameObject = _assets.Instantiate(path: prefabPath);

            if (registerProgressWatchers)
                RegisterProgressWatchers(gameObject);

            return gameObject;
        }

        private GameObject InstantiateRegistered(string prefabPath, Vector3 at, bool registerProgressWatchers = true)
        {
            GameObject gameObject = _assets.Instantiate(path: prefabPath, at: at);

            if (registerProgressWatchers)
                RegisterProgressWatchers(gameObject);

            return gameObject;
        }

        private T InstantiateRegistered<T>(T prefab, bool registerProgressWatchers = true) where T : MonoBehaviour
        {
            T monoBehaviour = Object.Instantiate<T>(prefab);

            if (registerProgressWatchers)
                RegisterProgressWatchers(monoBehaviour.gameObject);

            return monoBehaviour;
        }

        private T InstantiateRegistered<T>(T prefab, Vector3 at, bool registerProgressWatchers = true) where T : MonoBehaviour
        {
            T monoBehaviour = Object.Instantiate<T>(prefab, at, Quaternion.identity);

            if (registerProgressWatchers)
                RegisterProgressWatchers(monoBehaviour.gameObject);

            return monoBehaviour;
        }

        private GameObject InstantiateRegistered(GameObject prefab, Vector3 at, bool registerProgressWatchers = true)
        {
            GameObject go = Object.Instantiate(prefab, at, Quaternion.identity);

            if (registerProgressWatchers)
                RegisterProgressWatchers(go);

            return go;
        }

        private class CreatedByIdGameObjectsConstructor : ICreatedByIdGameObjectVisitor
        {
            private readonly GameFactory _gameFactory;

            public CreatedByIdGameObjectsConstructor(GameFactory gameFactory)
            {
                _gameFactory = gameFactory;
            }

            void ICreatedByIdGameObjectVisitor.Visit(TutorialOnly simpleObject)
            {
                simpleObject.Construct();
                GenerateIdIfApplicable(simpleObject);
            }

            void ICreatedByIdGameObjectVisitor.Visit(SellBoard sellBoard)
            {
                sellBoard.Construct(_gameFactory._uiMediator, _gameFactory._configs);
                GenerateIdIfApplicable(sellBoard);
            }

            void ICreatedByIdGameObjectVisitor.Visit(UpgradeBoard upgradeBoard)
            {
                upgradeBoard.Construct(_gameFactory._uiMediator, _gameFactory._configs, _gameFactory._progressService);
                GenerateIdIfApplicable(upgradeBoard);
            }

            void ICreatedByIdGameObjectVisitor.Visit(ResourceSource resourceSource)
            {
                resourceSource.Construct(_gameFactory._resourceFactory, _gameFactory._dropCountCalculatorService, _gameFactory._audio, _gameFactory._effectFactory);
                GenerateIdIfApplicable(resourceSource);
            }

            void ICreatedByIdGameObjectVisitor.Visit(ResourceStorage resourceStorage)
            {
                resourceStorage.Construct(_gameFactory._resourceFactory, _gameFactory._progressService, _gameFactory._audio, _gameFactory._effectFactory);
                GenerateIdIfApplicable(resourceStorage);
            }

            void ICreatedByIdGameObjectVisitor.Visit(Converter converter)
            {
                converter.Construct(_gameFactory._resourceFactory, _gameFactory._progressService, _gameFactory._audio, _gameFactory._effectFactory);
                converter.Init();
                GenerateIdIfApplicable(converter);
            }

            void ICreatedByIdGameObjectVisitor.Visit(Workbench workbench)
            {
                workbench.Construct(_gameFactory._resourceFactory, _gameFactory._toolFactory, _gameFactory._audio, _gameFactory._effectFactory);
                workbench.Init();
                GenerateIdIfApplicable(workbench);
            }

            void ICreatedByIdGameObjectVisitor.Visit(Workshop workshop)
            {
                workshop.Construct(_gameFactory._audio, _gameFactory._effectFactory, _gameFactory);
                workshop.Init();
                GenerateIdIfApplicable(workshop);
            }

            void ICreatedByIdGameObjectVisitor.Visit(Chunk chunk)
            {
                chunk.Construct(_gameFactory._audio, _gameFactory._effectFactory, _gameFactory);
                chunk.Init();
                GenerateIdIfApplicable(chunk);
            }

            void ICreatedByIdGameObjectVisitor.Visit(Dungeon dungeon)
            {
                dungeon.Construct(_gameFactory, _gameFactory._audio, _gameFactory._effectFactory, _gameFactory._progressService);

                dungeon.Spawn();
                GenerateIdIfApplicable(dungeon);
            }

            void ICreatedByIdGameObjectVisitor.Visit(FinalPrize finalPrize)
            {
                finalPrize.Construct(_gameFactory._audio);

                GenerateIdIfApplicable(finalPrize);
            }

            void ICreatedByIdGameObjectVisitor.Visit(BootsAdsObject boots)
            {
                boots.Construct(_gameFactory._adsService, _gameFactory._audio);

                GenerateIdIfApplicable(boots);
            }

            void ICreatedByIdGameObjectVisitor.Visit(AdsResourceBox adsResourceBox)
            {
                adsResourceBox.Construct(_gameFactory._adsService, _gameFactory._resourceFactory, _gameFactory._audio);
                adsResourceBox.Init();
                GenerateIdIfApplicable(adsResourceBox);
            }

            private void GenerateIdIfApplicable(MonoBehaviour monoBehaviour)
            {
                if (monoBehaviour.TryGetComponent(out UniqueId uniqueId))
                    uniqueId.GenerateId();
            }
        }
    }
}