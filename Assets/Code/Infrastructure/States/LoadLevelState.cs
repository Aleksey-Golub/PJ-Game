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
            _gameFactory.Cleanup();
            _sceneLoader.Load(sceneName, OnLoaded);
        }

        public void Exit() =>
          _loadingCurtain.Hide();

        private void OnLoaded()
        {
            string loadedSceneName = _loadingSceneName;
            InitProgressForLevel(loadedSceneName);

            InitUIRoot();
            InitGameWorld(loadedSceneName) ;
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
            //InitSpawners();
            //InitLootPieces();

            Hud hud = _gameFactory.CreateHud();
            InitUIMediator(hud);
            GameObject hero = _gameFactory.CreateHero(GameObject.FindWithTag(INITIAL_POINT_TAG));
            CameraFollow(hero);
        }

        private void InitDroppedResources(string loadedSceneName)
        {
            foreach (KeyValuePair<string, ResourceOnSceneData> item in _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[loadedSceneName].ResourcesDatas.ResourcesOnScene.Dictionary)
            {
                Resource resource = _resourceFactory.Get(item.Value.Position.AsUnityVector(), Quaternion.identity);
                resource.UniqueId.Id = item.Key;
                resource.Init(_configs.GetConfigFor(item.Value.Type), item.Value.Count);
            }
        }

        /*
        private void InitSpawners()
        {
            string sceneKey = SceneManager.GetActiveScene().name;
            LevelStaticData levelData = _configs.ForLevel(sceneKey);

            foreach (EnemySpawnerStaticData spawnerData in levelData.EnemySpawners)
                _gameFactory.CreateSpawner(spawnerData.Id, spawnerData.Position, spawnerData.MonsterTypeId);
        }
        *//*
        private void InitLootPieces()
        {
            foreach (KeyValuePair<string, LootPieceData> item in _progressService.Progress.WorldData.LootData.LootPiecesOnScene.Dictionary)
            {
                LootPiece lootPiece = _gameFactory.CreateLoot();
                lootPiece.GetComponent<UniqueId>().Id = item.Key;
                lootPiece.Initialize(item.Value.Loot);
                lootPiece.transform.position = item.Value.Position.AsUnityVector();
            }
        }
        */
        private void InitUIMediator(Hud hud)
        {
            _uIMediator.Init(hud);
        }
        
        private void CameraFollow(GameObject hero)
        {
            Camera.main.GetComponent<CameraMover>().SetTarget(hero.transform);
        }
    }
}