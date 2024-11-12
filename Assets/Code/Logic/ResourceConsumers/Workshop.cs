using Code.Services;
using UnityEngine;

[SelectionBase]
public class Workshop : SingleUseConsumerBase<ResourceConsumerView>
{
    [SerializeField] private SpawnData[] _spawnDatas;

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

    protected override void DropObject()
    {
        View.PlayDropResourceSound();
        View.ShowHitEffect();

        foreach (SpawnData data in _spawnDatas)
            Instantiate(data.Prefab, data.Point.position, Quaternion.identity);
    }
}
