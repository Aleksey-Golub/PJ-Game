using Code.Infrastructure;
using Code.Services;
using System.Collections;
using UnityEngine;

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

    private void Start()
    {
        var audio = AllServices.Container.Single<IAudioService>();
        var effectFactory = AllServices.Container.Single<IEffectFactory>();
        var gameFactory = AllServices.Container.Single<IGameFactory>();

        Construct(audio, effectFactory, gameFactory);
        Init();
    }

    public void Construct(IAudioService audio, IEffectFactory effectFactory, IGameFactory gameFactory)
    {
        Construct();
        _gameFactory = gameFactory;

        View.Construct(audio, effectFactory);
    }

    protected override Sprite GetGenerateObjSprite() => null;

    protected override void OnFilled()
    {
        OpenChainedChunks();

        base.OnFilled();
    }

    protected override void DropObject()
    {
        View.PlayDropResourceSound();
        View.ShowHitEffect();

        foreach (SpawnGameObjectData data in _spawnDatas)
            _gameFactory.GetGameObject(data.GameObjectId, at: data.Point.position);
    }

    private IEnumerator OpenDelayed()
    {
        yield return new WaitForSeconds(_openDelay);

        OnFilled();
    }

    private void OpenChainedChunks()
    {
        foreach (Chunk chunk in _chunksToOpen)
            chunk.StartCoroutine(chunk.OpenDelayed());
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        foreach (SpawnGameObjectData data in _spawnDatas)
        {
            Handles.Label(data.Point.position, data.GameObjectId);
        }
    }
#endif
}

