using Code.Infrastructure;
using Code.Services;
using UnityEngine;
using Code.Data;
using System.Linq;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SelectionBase]
public class Workshop : SingleUseConsumerBase<ResourceConsumerView>
{
    [SerializeField] private WorkshopType _type;
    [SerializeField] private SpawnGameObjectData[] _spawnDatas;

    private IGameFactory _gameFactory;
    private IPersistentProgressService _progressService;

    public WorkshopType Type => _type;

    protected override Action OnExhaustCallback => OnExhausted;
    protected override bool DisableSelfOnExhaused => false;

    #region EDITOR
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        foreach (SpawnGameObjectData data in _spawnDatas)
        {
            Vector3 pos = data.Point != null ? data.Point.position : data.Position;
            Handles.Label(pos, data.GameObjectId);
        }
    }
#endif
    #endregion

    private void Start()
    {
        if (SceneBuiltInItem)
        {
            var audio = AllServices.Container.Single<IAudioService>();
            var effectFactory = AllServices.Container.Single<IEffectFactory>();
            var gameFactory = AllServices.Container.Single<IGameFactory>();
            var progressService = AllServices.Container.Single<IPersistentProgressService>();

            Construct(audio, effectFactory, gameFactory, progressService);
            Init();

            gameFactory.RegisterProgressWatchersExternal(gameObject);
        }
    }

    public void Construct(IAudioService audio, IEffectFactory effectFactory, IGameFactory gameFactory, IPersistentProgressService progressService)
    {
        Construct();
        _gameFactory = gameFactory;
        _progressService = progressService;

        View.Construct(audio, effectFactory);
    }
    public void InitOnLoad(ResourceConfig needResourceConfig)
    {
        _needResourceConfig = needResourceConfig;
    }

    public override void WriteToProgress(GameProgress progress)
    {
        var workshopsOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].WorkshopsDatas.WorkshopsOnScene;

        // just to optimize
        workshopsOnScene.Dictionary.TryGetValue(Id, out var data);
        if (data != null && !HasChangesBetweenSavedStateAndCurrentState(data))
            return;

        workshopsOnScene.Dictionary[Id] = new WorkshopOnSceneData(
            transform.position.AsVectorData(),
            SceneBuiltInItem,
            _needResourceConfig.Type,
            _needResourceCount,
            CurrentNeedResourceCount,
            Available,

            _spawnDatas.Select(sd => new Code.Data.SpawnGameObjectData(sd.GameObjectId, sd.Point != null ? sd.Point.position.AsVectorData() : sd.Position.AsVectorData())).ToArray(),
            type: _type
            );
    }

    public override void ReadProgress(GameProgress progress)
    {
        var workshopsOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].WorkshopsDatas.WorkshopsOnScene;

        // we are build-in-scene object and it is first start of level
        if (!workshopsOnScene.Dictionary.TryGetValue(Id, out var myState))
            return;

        // restore state
        transform.position = myState.Position.AsUnityVector();
        _needResourceCount = myState.NeedResourceCount;
        CurrentNeedResourceCount = myState.CurrentNeedResourceCount;
        CurrentPreUpload = _needResourceCount - CurrentNeedResourceCount;

        _spawnDatas = myState.SpawnData.Select(sd => new SpawnGameObjectData() { GameObjectId = sd.GameObjectId, Position = sd.Position.AsUnityVector() }).ToArray();

        View.ShowNeeds(CurrentNeedResourceCount, _needResourceCount, Available);
        if (CurrentNeedResourceCount == 0)
        {
            View.ShowExhaust();
            ExhaustStrategy.ExhaustImmediately();
        }
    }

    protected override Sprite GetGenerateObjSprite() => null;

    protected override void DropObject()
    {
        View.PlayDropResourceSound();
        View.ShowHitEffect();

        foreach (SpawnGameObjectData data in _spawnDatas)
            _gameFactory.GetGameObject(data.GameObjectId, at: data.Point != null ? data.Point.position : data.Position);
    }

    private void OnExhausted()
    {
        if (SceneBuiltInItem)
        {
            gameObject.SetActive(false);
            return;
        }

        StartCoroutine(RemoveSelfCor());
    }

    private IEnumerator RemoveSelfCor()
    {
        yield return null;

        _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].WorkshopsDatas.WorkshopsOnScene.Dictionary.Remove(UniqueId.Id);
        _gameFactory.Recycle(gameObject);
    }

    protected override void Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}

public enum WorkshopType
{
    None = 0,
    WorkshopBase = 1,
    DryFruitBush = 2,
    PointForPlanting = 3,
    LittlePhoenixTree = 4,
    LittleBush = 5,
    LittleDesertTree = 6,
    LittlePoppyBush = 7,
    Bridge_Partial_CrackedSupportNorth = 50,
    //Bridge_Partial_CrackedSupportSouth = 51, // reserved
    Bridge_Partial_RestoredSupports = 53,
    Bridge_Partial_FirstRopes = 55,
    Bridge_Partial_WoodWithoutNails = 57,
    Bridge_Partial_WoodWithNails = 59,
}
