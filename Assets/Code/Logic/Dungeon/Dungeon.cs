using System.Collections.Generic;
using UnityEngine;
using Code.Services;
using Code.Data;
using Code.Infrastructure;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Dungeon : MonoBehaviour, ISavedProgressReader, ISavedProgressWriter, IUniqueIdHolder, IPossibleSceneBuiltInItem, ICreatedByIdGameObject
{
    [field: SerializeField] public bool SceneBuiltInItem { get; private set; }
    [field: SerializeField] public UniqueId UniqueId { get; private set; }
    [Space]
    [SerializeField] private DungeonSpawnData[] _spawnDatas;
    [SerializeField] private DungeonEntrance _entrance;
    [SerializeField] private DungeonEntrance _exit;

    private IGameFactory _gameFactory;
    private IPersistentProgressService _progressService;
    private List<ResourceSource> _spawnedResourceSources;

    private string Id => UniqueId.Id;

    #region EDITOR
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        foreach (DungeonSpawnData data in _spawnDatas)
        {
            foreach (Vector3 localPos in data.LocalPositions)
            {
                Vector3 pos = localPos + transform.position;
                Handles.Label(pos, data.ResourceSourceId);
            }
        }
    }
#endif
    #endregion

    private void Start()
    {
        if (SceneBuiltInItem)
        {
            var gameFactory = AllServices.Container.Single<IGameFactory>();
            var audio = AllServices.Container.Single<IAudioService>();
            var effectFactory = AllServices.Container.Single<IEffectFactory>();
            var progressService = AllServices.Container.Single<IPersistentProgressService>();

            Construct(gameFactory, audio, effectFactory, progressService);
            gameFactory.RegisterProgressWatchersExternal(gameObject);

            if (IsFirstStartOfLevel())
                Spawn();
        }

        bool IsFirstStartOfLevel()
        {
            var dungeonsOnScene = _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].DungeonsDatas.DungeonsOnScene;
            // we are build-in-scene object and it is first start of level
            if (!dungeonsOnScene.Dictionary.TryGetValue(Id, out var myState))
                return true;

            return false;
        }
    }

    public void Construct(IGameFactory gameFactory, IAudioService audio, IEffectFactory effectFactory, IPersistentProgressService progressService)
    {
        _gameFactory = gameFactory;
        _progressService = progressService;

        _entrance.Construct(audio, effectFactory);
        _exit.Construct(audio, effectFactory);

        _spawnedResourceSources = new List<ResourceSource>(_spawnDatas.Length);

        _exit.InteractedByPlayer += OnPlayerExit;
    }

    private void OnDestroy()
    {
        _exit.InteractedByPlayer -= OnPlayerExit;
    }

    public void WriteToProgress(GameProgress progress)
    {
        var dungeonsOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].DungeonsDatas.DungeonsOnScene;

        // just to optimize
        dungeonsOnScene.Dictionary.TryGetValue(Id, out var data);
        if (data != null && !HasChangesBetweenSavedStateAndCurrentState(data))
            return;

        dungeonsOnScene.Dictionary[Id] = new DungeonOnSceneData(
            transform.position.AsVectorData(),
            SceneBuiltInItem,
            spawnData: _spawnDatas.Select(sd => new Code.Data.DungeonSpawnData(sd.ResourceSourceId, sd.LocalPositions.Select(lp => lp.AsVectorData()).ToArray())).ToArray(),
            resourceSourcesIds: _spawnedResourceSources.Select(rs => rs.UniqueId.Id).ToArray(),
            entranceState: _entrance.SaveState(),
            exitState: _exit.SaveState()
            );
    }

    public void ReadProgress(GameProgress progress)
    {
        var dungeonsOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].DungeonsDatas.DungeonsOnScene;

        // we are build-in-scene object and it is first start of level
        if (!dungeonsOnScene.Dictionary.TryGetValue(Id, out var myState))
            return;

        // restore state
        transform.position = myState.Position.AsUnityVector();
        _spawnDatas = myState.SpawnData.Select(sd => new DungeonSpawnData() { ResourceSourceId = sd.ResourceSourceGameObjectId, LocalPositions = sd.LocalPositions.Select(lp => lp.AsUnityVector()).ToArray() }).ToArray();

        _entrance.RestoreState(myState.EntranceState);
        _exit.RestoreState(myState.ExitState);

        RestoreResourceSources(myState.ResourceSourcesIds);
    }

    private void RestoreResourceSources(string[] resourceSourcesIds)
    {
        foreach (string rsId in resourceSourcesIds)
        {
            foreach (ISavedProgressWriter item in _gameFactory.ProgressWriters)
            {
                if (item is ResourceSource resourceSource && resourceSource.UniqueId.Id == rsId)
                {
                    resourceSource.Dropped += ResourceSourceDropped;
                    _spawnedResourceSources.Add(resourceSource);
                }
            }
        }
    }

    public void Spawn()
    {
        foreach (DungeonSpawnData data in _spawnDatas)
        {
            foreach (var localPos in data.LocalPositions)
            {
                ResourceSource r = _gameFactory.GetGameObject(data.ResourceSourceId, localPos + transform.position).GetComponent<ResourceSource>();

                r.Dropped += ResourceSourceDropped;

                _spawnedResourceSources.Add(r);
            }
        }
    }

    private void OnPlayerExit(DungeonEntrance exit)
    {
        _entrance.ReStart();
        Spawn();
        _exit.ForceClose();
    }

    private void ResourceSourceDropped(ResourceSource r)
    {
        r.Dropped -= ResourceSourceDropped;
        _spawnedResourceSources.Remove(r);

        _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].ResourceSourcesDatas.ResourceSourcesOnScene.Dictionary.Remove(r.UniqueId.Id);
        _gameFactory.Recycle(r.gameObject);

        if (_spawnedResourceSources.Count == 0)
            OpenExit();
    }

    private void OpenExit()
    {
        _exit.ForceOpen();
    }

    private bool HasChangesBetweenSavedStateAndCurrentState(DungeonOnSceneData data)
    {
        // always return true because of the lack of control over resource sources
        return true;
    }

    void ICreatedByIdGameObject.Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);

    [System.Serializable]
    internal struct DungeonSpawnData
    {
        [GameObjectIdHolder]
        public string ResourceSourceId;
        public Vector3[] LocalPositions;
    }
}
