using System.Collections.Generic;
using Code.Services;
using Code.UI;
using Code.UI.Services;
using UnityEngine;

namespace Code.Infrastructure
{
    public class GameFactory : IGameFactory, IPlayerProvider
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
        private Player _hero;

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

        public Player GetPlayer() => _hero;

        public GameObject CreateHero(GameObject at)
        {
            _hero = InstantiateRegistered(AssetPath.HERO_PATH, at.transform.position).GetComponent<Player>();
            _hero.Construct(_input, _uiMediator.GetPlayerInventoryView(), _configs, _popupFactory, _transitionalResourceFactory, _progressService);

            return _hero.gameObject;
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

            resourceSource.Construct(_resourceFactory, _dropCountCalculatorService, _audio, _effectFactory, this, _progressService);

            return resourceSource;
        }

        public ResourceStorage CreateResourceStorage(ResourceStorageType type, Vector3 at)
        {
            ResourceStorageMatcher rStorageMatcher = _configs.GetMatcherFor(type);
            ResourceStorage resourceStorage = InstantiateRegistered(rStorageMatcher.Template, at);

            resourceStorage.Construct(_resourceFactory, _progressService, _audio, _effectFactory, this, this);

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
                    (simpleObject as FirstPrize).Construct(_audio, _effectFactory, this, _progressService, this);
                    break;
                case SimpleObjectType.Prize_Second:
                    (simpleObject as FinalPrize).Construct(_audio);
                    break;
                case SimpleObjectType.Prize_Last:
                    (simpleObject as FinalPrize).Construct(_audio);
                    break;
                case SimpleObjectType.Boots:
                    (simpleObject as BootsAdsObject).Construct(_adsService, _audio);
                    break;
                case SimpleObjectType.AdsResourceBox:
                    (simpleObject as AdsResourceBox).Construct(_adsService, _resourceFactory, _audio);
                    break;
                case SimpleObjectType.Portal_1_2:
                case SimpleObjectType.Portal_2_1:
                    (simpleObject as Portal).Construct(_audio);
                    break;
                case SimpleObjectType.BridgeCompleted_Wood_Nails_2_Ropes:
                    (simpleObject as SimpleObject).Construct();
                    break;
                case SimpleObjectType.Desert_Tornado:
                    (simpleObject as SimpleObject).Construct();
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
            workbench.Construct(_resourceFactory, _toolFactory, _audio, _effectFactory, this, _progressService);

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
                case WorkshopType.PointForPlanting:
                    assetPath = AssetPath.POINT_FOR_PLANTING_PATH;
                    break;
                case WorkshopType.LittlePhoenixTree:
                    assetPath = AssetPath.LITTLE_PHOENIX_TREE_PATH;
                    break;
                case WorkshopType.LittleBush:
                    assetPath = AssetPath.LITTLE_BUSH_PATH;
                    break;
                case WorkshopType.LittleDesertTree:
                    assetPath = AssetPath.LITTLE_DESERT_TREE_PATH;
                    break;
                case WorkshopType.LittlePoppyBush:
                    assetPath = AssetPath.LITTLE_POPPY_BUSH_PATH;
                    break;
                case WorkshopType.Bridge_Partial_CrackedSupportNorth:
                    assetPath = AssetPath.BRIDGE_PARTIAL_CRACKEDSUPPORTS_NORTH_PATH;
                    break;
                case WorkshopType.Bridge_Partial_CrackedSupportSouth:
                    assetPath = AssetPath.BRIDGE_PARTIAL_CRACKEDSUPPORTS_SOUTH_PATH;
                    break;
                case WorkshopType.Bridge_Partial_RestoredSupports:
                    assetPath = AssetPath.BRIDGE_PARTIAL_RESTOREDSUPPORTS_PATH;
                    break;
                case WorkshopType.Bridge_Partial_FirstRopes:
                    assetPath = AssetPath.BRIDGE_PARTIAL_FIRSTROPES_PATH;
                    break;
                case WorkshopType.Bridge_Partial_WoodWithoutNails:
                    assetPath = AssetPath.BRIDGE_PARTIAL_WOODWITHOUTNAILS_PATH;
                    break;
                case WorkshopType.Bridge_Partial_WoodWithNails:
                    assetPath = AssetPath.BRIDGE_PARTIAL_WOODWITHNAILS_PATH;
                    break;
                case WorkshopType.Special_Second_Prize_Spawner:
                    assetPath = AssetPath.SPECIAL_SECOND_PRIZE_SPAWNER_PATH;
                    break;
                case WorkshopType.None:
                default:
                    throw new System.NotImplementedException($"[GameFactory] CreateWorkshop(). Not implemented for {type}");
            }

            Workshop workshop = InstantiateRegistered(assetPath, at).GetComponent<Workshop>();
            workshop.Construct(_audio, _effectFactory, this, _progressService);

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
                case ConverterType.FurnaceCoalConverter:
                    assetPath = AssetPath.FURNACE_COAL_PATH;
                    break;
                case ConverterType.FurnaceIronConverter:
                    assetPath = AssetPath.FURNACE_IRON_PATH;
                    break;
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

            tutorial.Construct(this, _progressService, _hero.GetComponent<Player>());

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

            _hero = null;
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

            void ICreatedByIdGameObjectVisitor.Visit(SimpleObject simpleObject)
            {
                simpleObject.Construct();
                GenerateIdIfApplicable(simpleObject);
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
                resourceSource.Construct(
                    _gameFactory._resourceFactory, 
                    _gameFactory._dropCountCalculatorService, 
                    _gameFactory._audio, 
                    _gameFactory._effectFactory,
                    _gameFactory,
                    _gameFactory._progressService
                    );
                GenerateIdIfApplicable(resourceSource);
            }

            void ICreatedByIdGameObjectVisitor.Visit(ResourceStorage resourceStorage)
            {
                resourceStorage.Construct(
                    _gameFactory._resourceFactory, 
                    _gameFactory._progressService, 
                    _gameFactory._audio, 
                    _gameFactory._effectFactory, 
                    _gameFactory,
                    _gameFactory
                    );
                GenerateIdIfApplicable(resourceStorage);
            }

            void ICreatedByIdGameObjectVisitor.Visit(Converter converter)
            {
                converter.Construct(_gameFactory._resourceFactory, _gameFactory._progressService, _gameFactory._audio, _gameFactory._effectFactory);
                converter.Init();
                GenerateIdIfApplicable(converter);

                Metrika.ConverterBought(converter.Type);
            }

            void ICreatedByIdGameObjectVisitor.Visit(Workbench workbench)
            {
                workbench.Construct(
                    _gameFactory._resourceFactory, 
                    _gameFactory._toolFactory, 
                    _gameFactory._audio, 
                    _gameFactory._effectFactory,
                    _gameFactory,
                    _gameFactory._progressService
                    );
                workbench.Init();
                GenerateIdIfApplicable(workbench);
            }

            void ICreatedByIdGameObjectVisitor.Visit(Workshop workshop)
            {
                workshop.Construct(
                    _gameFactory._audio, 
                    _gameFactory._effectFactory, 
                    _gameFactory, 
                    _gameFactory._progressService);
                workshop.Init();
                GenerateIdIfApplicable(workshop);

                Metrika.WorkshopBuilt(workshop.Type);
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
            
            void ICreatedByIdGameObjectVisitor.Visit(Portal portal)
            {
                portal.Construct(_gameFactory._audio);
                GenerateIdIfApplicable(portal);
            }

            void ICreatedByIdGameObjectVisitor.Visit(FirstPrize finalPrize)
            {
                finalPrize.Construct(
                    _gameFactory._audio,
                    _gameFactory._effectFactory,
                    _gameFactory,
                    _gameFactory._progressService,
                    _gameFactory
                    );

                GenerateIdIfApplicable(finalPrize);
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