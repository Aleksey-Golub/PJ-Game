using Code.Infrastructure;
using Code.Services;
using UnityEngine;

[SelectionBase]
public class Workshop : SingleUseConsumerBase<ResourceConsumerView>, ICreatedByIdGameObject
{
    [SerializeField] private SpawnGameObjectData[] _spawnDatas;

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

    protected override void DropObject()
    {
        View.PlayDropResourceSound();
        View.ShowHitEffect();

        foreach (SpawnGameObjectData data in _spawnDatas)
            _gameFactory.GetGameObject(data.GameObjectId, at: data.Point.position);
    }

    void ICreatedByIdGameObject.Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}
