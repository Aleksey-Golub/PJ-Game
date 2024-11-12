using Code.Services;
using System.Collections;
using UnityEngine;

[SelectionBase]
public class Chunk : SingleUseConsumerBase<ChunkView>
{
    [Space()]
    [SerializeField] private bool _openByOtherOnly;
    [SerializeField] private SpawnData[] _spawnDatas;
    [SerializeField] private Chunk[] _chunksToOpen;
    [SerializeField] private float _openDelay = 0.5f;

    private void Start()
    {
        var audio = AllServices.Container.Single<IAudioService>();
        var effectFactory = AllServices.Container.Single<IEffectFactory>();

        Construct(audio, effectFactory);
        Init();
    }

    private void Construct(IAudioService audio, IEffectFactory effectFactory)
    {
        Construct();

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

        foreach (SpawnData data in _spawnDatas)
            Instantiate(data.Prefab, data.Point.position, Quaternion.identity);
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
}

