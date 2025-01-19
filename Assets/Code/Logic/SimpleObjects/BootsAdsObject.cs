using Code.Infrastructure;
using Code.Services;
using UnityEngine;

public class BootsAdsObject : AdsObjectBase<AdsObjectView>
{
    [Space]
    [SerializeField] private float _boostSpeed = 5f;
    [SerializeField] private float _boostTime = 180f;
    [SerializeField] private string _format = "mm':'ss";

    private const string KEY = "k_boots_bonus_text";

    private void Start()
    {
        if (SceneBuiltInItem)
        {
            var adsService = AllServices.Container.Single<IAdsService>();
            var audio = AllServices.Container.Single<IAudioService>();
            var gameFactory = AllServices.Container.Single<IGameFactory>();

            Construct(adsService, audio);
            gameFactory.RegisterProgressWatchersExternal(gameObject);
        }
    }

    internal new void Construct(IAdsService adsService, IAudioService audio)
    {
        base.Construct(adsService, audio);

        LService.LanguageChanged += OnLanguageChanged;
        OnLanguageChanged();
    }

    private void OnDestroy()
    {
        LService.LanguageChanged -= OnLanguageChanged;
    }

    protected override void OnRewardedVideoEndSuccessfully()
    {
        base.OnRewardedVideoEndSuccessfully();

        //Logger.Log($"[Boots] {gameObject.name} - do speed");
        Player.SpeedUp(_boostSpeed, _boostTime);
    }

    private void OnLanguageChanged()
    {
        View.SetBonusText(string.Format(LService.Localize(KEY), _boostTime.ToStringFormated(_format)));
        //View.SetBonusText(string.Format(LService.Localize(KEY), _boostTime.ToStringDynamicFormated()));
    }

    protected override void Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}
