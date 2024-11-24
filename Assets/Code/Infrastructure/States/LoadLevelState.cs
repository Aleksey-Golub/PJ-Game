using Code.Data;
using Code.Services;
using Code.UI.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Infrastructure
{
    public class LoadLevelState : IPayloadedState<string>
    {
        private const string INITIAL_POINT_TAG = "PlayerInitialPoint";

        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly IGameFactory _gameFactory;
        private readonly IResourceFactory _resourceFactory;
        private readonly IEffectFactory _effectFactory;
        private readonly IPopupFactory _popupFactory;
        private readonly IToolFactory _toolFactory;
        private readonly ITransitionalResourceFactory _transitionalResourceFactory;
        private readonly IPersistentProgressService _progressService;
        private readonly IUIFactory _uiFactory;
        private readonly IUIMediator _uIMediator;
        private readonly IAudioService _audio;
        private readonly IConfigsService _configs;
        private string _loadingSceneName;

        public LoadLevelState(
            GameStateMachine gameStateMachine,
            SceneLoader sceneLoader,
            LoadingCurtain loadingCurtain,
            IGameFactory gameFactory,
            IResourceFactory resourceFactory,
            IEffectFactory effectFactory,
            IPopupFactory popupFactory,
            IToolFactory toolFactory,
            ITransitionalResourceFactory transitionalResourceFactory,
            IPersistentProgressService progressService,
            IUIFactory uiFactory,
            IUIMediator uIMediator,
            IAudioService audio,
            IConfigsService configs
            )
        {
            _stateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
            _gameFactory = gameFactory;
            _resourceFactory = resourceFactory;
            _effectFactory = effectFactory;
            _popupFactory = popupFactory;
            _toolFactory = toolFactory;
            _transitionalResourceFactory = transitionalResourceFactory;
            _progressService = progressService;
            _uiFactory = uiFactory;
            _uIMediator = uIMediator;
            _audio = audio;
            _configs = configs;
        }

        public void Enter(string sceneName)
        {
            _loadingSceneName = sceneName;

            _loadingCurtain.Show();

            _resourceFactory.Load();
            _effectFactory.Load();
            _popupFactory.Load();
            _toolFactory.Load();
            _transitionalResourceFactory.Load();

            InitProgressForLevel(sceneName);

            _sceneLoader.Load(sceneName, OnLoaded);
        }

        public void Exit() =>
          _loadingCurtain.Hide();

        private void OnLoaded()
        {
            string loadedSceneName = _loadingSceneName;

            InitUIRoot();
            InitGameWorld(loadedSceneName);
            InitTutorial(loadedSceneName);
            InitTutorialObjects(loadedSceneName);
            InformProgressReaders();
            _audio.PlayAmbient();
            _loadingSceneName = null;

            _stateMachine.Enter<GameLoopState>();
        }

        private void InitProgressForLevel(string sceneName)
        {
            Data.LevelsDatasDictionary levelsDatasDictionary = _progressService.Progress.WorldProgress.LevelsDatasDictionary;
            if (!levelsDatasDictionary.Dictionary.ContainsKey(sceneName))
                levelsDatasDictionary.Dictionary.Add(sceneName, new Data.LevelData(sceneName));
        }

        private void InitUIRoot() =>
          _uiFactory.CreateUIRoot();

        private void InformProgressReaders()
        {
            foreach (ISavedProgressReader progressReader in _gameFactory.ProgressReaders)
                progressReader.ReadProgress(_progressService.Progress);
        }

        private void InitGameWorld(string loadedSceneName)
        {
            InitDroppedResources(loadedSceneName);
            InitDroppedTools(loadedSceneName);
            InitResourceSources(loadedSceneName);
            InitResourceStorages(loadedSceneName);
            InitSimpleObjects(loadedSceneName);
            InitAdsBoxesObjects(loadedSceneName);
            InitWorkbenches(loadedSceneName);
            InitChunks(loadedSceneName);
            InitWorkshops(loadedSceneName);
            InitConverters(loadedSceneName);
            InitDungeons(loadedSceneName);

            Hud hud = _gameFactory.CreateHud();
            InitUIMediator(hud);
            GameObject hero = _gameFactory.CreateHero(GameObject.FindWithTag(INITIAL_POINT_TAG));
            CameraFollow(hero);
        }

        private void InitTutorial(string loadedSceneName)
        {
            var tutorialProgress = _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[loadedSceneName].TutorialProgress;

            if (tutorialProgress.IsCompleted)
                return;

            // There is no tutorial for this scene
            // This is valid state
            if (!_configs.TutorialsMatchers.ContainsKey(loadedSceneName))
            {
                tutorialProgress.IsCompleted = true;
                tutorialProgress.Stage = -10;
                return;
            }

            Tutorial tutorial = _gameFactory.CreateTutorial(loadedSceneName);
        }

        private void InitTutorialObjects(string loadedSceneName)
        {
            foreach (var item in _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[loadedSceneName].TutorialProgress.TutorialObjectsDatas.TutorialObjectsOnScene.Dictionary)
            {
                if (item.Value.SceneBuiltInItem)
                    continue;

                Vector3 position = item.Value.Position.AsUnityVector();
                TutorialOnly tutorialOnly = _gameFactory.GetGameObject(item.Value.GameObjectId, position).GetComponent<TutorialOnly>();
                tutorialOnly.UniqueId.Id = item.Key;
            }
        }

        private void InitDungeons(string loadedSceneName)
        {
            foreach (KeyValuePair<string, DungeonOnSceneData> item in _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[loadedSceneName].DungeonsDatas.DungeonsOnScene.Dictionary)
            {
                if (item.Value.SceneBuiltInItem)
                    continue;

                Vector3 position = item.Value.Position.AsUnityVector();
                Dungeon dungeon = _gameFactory.CreateDungeon(item.Value.GameObjectId, position);
                dungeon.UniqueId.Id = item.Key;
            }
        }

        private void InitConverters(string loadedSceneName)
        {
            foreach (KeyValuePair<string, ConverterOnSceneData> item in _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[loadedSceneName].ConvertersDatas.ConvertersOnScene.Dictionary)
            {
                if (item.Value.SceneBuiltInItem)
                    continue;

                Vector3 position = item.Value.Position.AsUnityVector();
                Converter converter = _gameFactory.CreateConverter(item.Value.Type, position);
                converter.UniqueId.Id = item.Key;

                converter.Init();
            }
        }

        private void InitWorkshops(string loadedSceneName)
        {
            foreach (KeyValuePair<string, WorkshopOnSceneData> item in _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[loadedSceneName].WorkshopsDatas.WorkshopsOnScene.Dictionary)
            {
                if (item.Value.SceneBuiltInItem)
                    continue;

                Vector3 position = item.Value.Position.AsUnityVector();
                Workshop workshop = _gameFactory.CreateWorkshop(item.Value.Type, position);
                workshop.UniqueId.Id = item.Key;

                workshop.InitOnLoad(_configs.GetConfigFor(item.Value.NeedResourceType));
                workshop.Init();
            }
        }

        private void InitChunks(string loadedSceneName)
        {
            foreach (KeyValuePair<string, ChunkOnSceneData> item in _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[loadedSceneName].ChunksDatas.ChunksOnScene.Dictionary)
            {
                if (item.Value.SceneBuiltInItem)
                    continue;

                Vector3 position = item.Value.Position.AsUnityVector();
                Chunk chunk = _gameFactory.CreateChunk(position);
                chunk.UniqueId.Id = item.Key;

                chunk.InitOnLoad(_configs.GetConfigFor(item.Value.NeedResourceType));
                chunk.Init();
            }
        }

        private void InitWorkbenches(string loadedSceneName)
        {
            foreach (KeyValuePair<string, WorkbenchOnSceneData> item in _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[loadedSceneName].WorkbenchesDatas.WorkbenchesOnScene.Dictionary)
            {
                if (item.Value.SceneBuiltInItem)
                    continue;

                Vector3 position = item.Value.Position.AsUnityVector();
                Workbench workbench = _gameFactory.CreateWorkbench(position);
                workbench.UniqueId.Id = item.Key;

                IDropObjectConfig dropConfig =
                    item.Value.DropResourceType != ResourceType.None ?
                        _configs.GetConfigFor(item.Value.DropResourceType) :
                        item.Value.DropToolType != ToolType.None ?
                            _configs.GetConfigFor(item.Value.DropToolType) :
                            throw new System.ArgumentException("No one valid drop object config found");

                workbench.InitOnLoad(_configs.GetConfigFor(item.Value.NeedResourceType), dropConfig);
                workbench.Init();
            }
        }

        private void InitSimpleObjects(string loadedSceneName)
        {
            foreach (KeyValuePair<string, SimpleObjectOnSceneData> item in _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[loadedSceneName].SimpleObjectsDatas.SimpleObjectsOnScene.Dictionary)
            {
                if (item.Value.SceneBuiltInItem)
                    continue;

                Vector3 position = item.Value.Position.AsUnityVector();

                if (item.Value.Type == SimpleObjectType.TutorialOnly)
                {
                    Logger.LogError($"[LoadLevelState] InitSimpleObjects() not supported for {item.Value.Type}");
                    continue;
                }

                SimpleObjectBase sObject = _gameFactory.CreateSimpleObject(item.Value.Type, position);

                sObject.UniqueId.Id = item.Key;
            }
        }

        private void InitAdsBoxesObjects(string loadedSceneName)
        {
            foreach (KeyValuePair<string, AdsBoxOnSceneData> item in _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[loadedSceneName].AdsBoxesDatas.AdsBoxesOnScene.Dictionary)
            {
                if (item.Value.SceneBuiltInItem)
                    continue;

                Vector3 position = item.Value.Position.AsUnityVector();
                AdsResourceBox adsBox = _gameFactory.CreateSimpleObject(item.Value.Type, position) as AdsResourceBox;
                adsBox.UniqueId.Id = item.Key;

                adsBox.InitOnLoad(_configs.GetConfigFor(item.Value.DropResourceType), item.Value.DropResourceCount);
                adsBox.Init();
            }
        }

        private void InitResourceStorages(string loadedSceneName)
        {
            foreach (KeyValuePair<string, ResourceStorageOnSceneData> item in _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[loadedSceneName].ResourceStoragesDatas.ResourceStoragesOnScene.Dictionary)
            {
                if (item.Value.SceneBuiltInItem)
                    continue;

                Vector3 position = item.Value.Position.AsUnityVector();
                ResourceStorage rStorage = _gameFactory.CreateResourceStorage(item.Value.Type, position);
                rStorage.UniqueId.Id = item.Key;

                if (item.Value.Type is ResourceStorageType.Chest)
                    rStorage.InitOnLoad(_configs.GetConfigFor(item.Value.DropResourceType));
            }
        }

        private void InitResourceSources(string loadedSceneName)
        {
            foreach (KeyValuePair<string, ResourceSourceOnSceneData> item in _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[loadedSceneName].ResourceSourcesDatas.ResourceSourcesOnScene.Dictionary)
            {
                if (item.Value.SceneBuiltInItem)
                    continue;

                Vector3 position = item.Value.Position.AsUnityVector();
                ResourceSource rSource = _gameFactory.CreateResourceSource(item.Value.Type, position);
                rSource.UniqueId.Id = item.Key;

                if (item.Value.Type is ResourceSourceType.Pot)
                    rSource.InitOnLoad(_configs.GetConfigFor(item.Value.DropResourceType), item.Value.DropResourceCount);
            }
        }

        private void InitDroppedResources(string loadedSceneName)
        {
            foreach (KeyValuePair<string, ResourceOnSceneData> item in _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[loadedSceneName].ResourcesDatas.ResourcesOnScene.Dictionary)
            {
                Vector3 position = item.Value.Position.AsUnityVector();
                int count = item.Value.Count;

                Resource resource = _resourceFactory.Get(position, Quaternion.identity);
                resource.UniqueId.Id = item.Key;
                resource.Init(_configs.GetConfigFor(item.Value.Type), count);
                resource.MoveAfterDrop(new DropData(0f, position, count));
            }
        }

        private void InitDroppedTools(string loadedSceneName)
        {
            foreach (KeyValuePair<string, ToolOnSceneData> item in _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[loadedSceneName].ToolsDatas.ToolsOnScene.Dictionary)
            {
                Vector3 position = item.Value.Position.AsUnityVector();

                Tool tool = _toolFactory.Get(position, Quaternion.identity);
                tool.UniqueId.Id = item.Key;
                tool.Init(_configs.GetConfigFor(item.Value.Type));
                tool.MoveAfterDrop(new DropData(0f, position, 1));
            }
        }

        private void InitUIMediator(Hud hud)
        {
            _uIMediator.Init(hud);
        }

        private void CameraFollow(GameObject hero)
        {
            Camera.main.GetComponent<TargetFollower>().SetTarget(hero.transform);
        }
    }
}