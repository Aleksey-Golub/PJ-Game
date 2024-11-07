﻿using Code.Data;
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
            InitDroppedTools(loadedSceneName);
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