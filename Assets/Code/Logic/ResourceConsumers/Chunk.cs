using Code.Infrastructure;
using Code.Services;
using System.Collections;
using UnityEngine;
using Code.Data;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SelectionBase]
public class Chunk : SingleUseConsumerBase<ChunkView>
{
    [Space()]
    [SerializeField] private bool _openByOtherOnly;
    [SerializeField] private SpawnGameObjectData[] _spawnDatas;
    [SerializeField] private Chunk[] _chunksToOpen;
    [SerializeField] private float _openDelay = 0.5f;

    private IGameFactory _gameFactory;
    private bool _opened;
    private bool _delayedOpenStart;
    private float _delayedOpenElapsedTime;

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
        var chunksOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].ChunksDatas.ChunksOnScene;

        // just to optimize
        chunksOnScene.Dictionary.TryGetValue(Id, out var data);
        if (data != null && !HasChangesBetweenSavedStateAndCurrentState(data))
            return;

        chunksOnScene.Dictionary[Id] = new ChunkOnSceneData(
            transform.position.AsVectorData(),
            SceneBuiltInItem,
            _needResourceConfig.Type,
            _needResourceCount,
            CurrentNeedResourceCount,

            _spawnDatas.Select(sd => new Code.Data.SpawnGameObjectData(sd.GameObjectId, sd.Point != null ? sd.Point.position.AsVectorData() : sd.Position.AsVectorData())).ToArray(),
            opened: _opened,
            openByOtherOnly: _openByOtherOnly,
            delayedOpenStart: _delayedOpenStart,
            delayedOpenElapsedTime: _delayedOpenElapsedTime
            );
    }

    public override void ReadProgress(GameProgress progress)
    {
        var chunksOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].ChunksDatas.ChunksOnScene;

        // we are in scene object and it is first start of level
        if (!chunksOnScene.Dictionary.TryGetValue(Id, out var myState))
            return;

        // restore state
        transform.position = myState.Position.AsUnityVector();
        _needResourceCount = myState.NeedResourceCount;
        CurrentNeedResourceCount = myState.CurrentNeedResourceCount;
        CurrentPreUpload = _needResourceCount - CurrentNeedResourceCount;

        _spawnDatas = myState.SpawnData.Select(sd => new SpawnGameObjectData() { GameObjectId = sd.GameObjectId, Position = sd.Position.AsUnityVector() }).ToArray();
        _opened = myState.Opened;
        _openByOtherOnly = myState.OpenByOtherOnly;
        _delayedOpenStart = myState.DelayedOpenStart;
        _delayedOpenElapsedTime = myState.DelayedOpenElapsedTime;

        View.ShowNeeds(CurrentNeedResourceCount, _needResourceCount);
        if (_opened)
        {
            View.ShowExhaust();
            ExhaustStrategy.ExhaustImmediately();
        }
        else if (_delayedOpenStart)
        {
            StartCoroutine(OpenDelayed());
        }
    }

    protected override Sprite GetGenerateObjSprite() => null;

    protected override void OnFilled()
    {
        OpenChainedChunks();

        base.OnFilled();

        _opened = true;
    }

    protected override void DropObject()
    {
        View.PlayDropResourceSound();
        View.ShowHitEffect();

        foreach (SpawnGameObjectData data in _spawnDatas)
            _gameFactory.GetGameObject(data.GameObjectId, at: data.Point != null ? data.Point.position : data.Position);
    }

    protected override bool HasChangesBetweenSavedStateAndCurrentState(SingleUseConsumerBaseOnScene data)
    {
        var myData = data as ChunkOnSceneData;

        return
            base.HasChangesBetweenSavedStateAndCurrentState(myData) ||
            myData.Opened != _opened ||
            myData.DelayedOpenStart != _delayedOpenStart ||
            myData.DelayedOpenElapsedTime != _delayedOpenElapsedTime
            ;
    }

    private IEnumerator OpenDelayed()
    {
        _delayedOpenStart = true;

        while (_delayedOpenElapsedTime < _openDelay)
        {
            yield return null;
            _delayedOpenElapsedTime += Time.deltaTime;
        }

        _delayedOpenStart = false;

        OnFilled();
    }

    private void OpenChainedChunks()
    {
        foreach (Chunk chunk in _chunksToOpen)
            chunk.StartCoroutine(chunk.OpenDelayed());
    }

    protected override void Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}

