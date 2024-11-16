using Code.Infrastructure;
using Code.Services;
using UnityEngine;
using Code.Data;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SelectionBase]
public class Workshop : SingleUseConsumerBase<ResourceConsumerView>
{
    [SerializeField] private WorkshopType _type;
    [SerializeField] private SpawnGameObjectData[] _spawnDatas;

    private IGameFactory _gameFactory;

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

            Construct(audio, effectFactory, gameFactory);
            Init();

            gameFactory.RegisterProgressWatchersExternal(gameObject);
        }
    }

    public void Construct(IAudioService audio, IEffectFactory effectFactory, IGameFactory gameFactory)
    {
        Construct();
        _gameFactory = gameFactory;

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

            _spawnDatas.Select(sd => new Code.Data.SpawnGameObjectData(sd.GameObjectId, sd.Point != null ? sd.Point.position.AsVectorData() : sd.Position.AsVectorData())).ToArray(),
            type: _type
            );
    }

    public override void ReadProgress(GameProgress progress)
    {
        var workshopsOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].WorkshopsDatas.WorkshopsOnScene;

        // we are in scene object and it is first start of level
        if (!workshopsOnScene.Dictionary.TryGetValue(Id, out var myState))
            return;

        // restore state
        transform.position = myState.Position.AsUnityVector();
        _needResourceCount = myState.NeedResourceCount;
        CurrentNeedResourceCount = myState.CurrentNeedResourceCount;
        CurrentPreUpload = _needResourceCount - CurrentNeedResourceCount;

        _spawnDatas = myState.SpawnData.Select(sd => new SpawnGameObjectData() { GameObjectId = sd.GameObjectId, Position = sd.Position.AsUnityVector() }).ToArray();

        View.ShowNeeds(CurrentNeedResourceCount, _needResourceCount);
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

    protected override void Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}

public enum WorkshopType
{
    None = 0,
    WorkshopBase = 1,
    DryFruitBush = 2,
}
