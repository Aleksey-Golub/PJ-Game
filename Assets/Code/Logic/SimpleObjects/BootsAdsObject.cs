using Code.Infrastructure;
using Code.Services;
using UnityEngine;

public class BootsAdsObject : AdsObjectBase<AdsObjectView>
{
    [Space]
    [SerializeField] private float _boostSpeed = 5f;
    [SerializeField] private float _boostTime = 180f;

    private void Start()
    {
        if (SceneBuiltInItem)
        {
            var adsService = AllServices.Container.Single<IAdsService>();
            var gameFactory = AllServices.Container.Single<IGameFactory>();

            Construct(adsService);
            gameFactory.RegisterProgressWatchersExternal(gameObject);
        }
    }

    internal new void Construct(IAdsService adsService) => base.Construct(adsService);

    protected override void OnRewardedVideoEndSuccessfully()
    {
        base.OnRewardedVideoEndSuccessfully();

        //Logger.Log($"[Boots] {gameObject.name} - do speed");
        Player.SpeedUp(_boostSpeed, _boostTime);
    }
    protected override void Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}
