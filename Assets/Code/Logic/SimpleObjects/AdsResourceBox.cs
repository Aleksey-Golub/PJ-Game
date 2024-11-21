using Code.Data;
using Code.Infrastructure;
using Code.Services;
using UnityEngine;

public class AdsResourceBox : AdsObjectBase<AdsBoxView>
{
    [Space]
    [SerializeField] private ResourceConfig _resourceConfig;
    [SerializeField, Min(1)] private int _dropResourceCount = 1;
    [field: SerializeField] public DropSettings DropSettings { get; private set; } = DropSettings.Default;

    private IResourceFactory _resourceFactory;

    private void Start()
    {
        if (SceneBuiltInItem)
        {
            var adsService = AllServices.Container.Single<IAdsService>();
            var resourceFactory = AllServices.Container.Single<IResourceFactory>();
            var audio = AllServices.Container.Single<IAudioService>();
            var gameFactory = AllServices.Container.Single<IGameFactory>();

            Construct(adsService, resourceFactory, audio);
            Init();

            gameFactory.RegisterProgressWatchersExternal(gameObject);
        }
    }

    public void Construct(
        IAdsService adsService,
        IResourceFactory resourceFactory,
        IAudioService audio
        )
    {
        _resourceFactory = resourceFactory;

        View.Construct(audio);

        Construct(adsService);
    }

    public void Init()
    {
        View.Init(dropResourceSprite: _resourceConfig.Sprite, dropResourceCount : _dropResourceCount);
    }

    public void InitOnLoad(ResourceConfig resourceConfig, int dropResourceCount)
    {
        _resourceConfig = resourceConfig;
        _dropResourceCount = dropResourceCount;
    }

    public override void WriteToProgress(GameProgress progress)
    {
        var adsBoxDataOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].AdsBoxesDatas.AdsBoxesOnScene;

        // just to optimize
        adsBoxDataOnScene.Dictionary.TryGetValue(Id, out var data);
        if (data != null && !HasChangesBetweenSavedStateAndCurrentState(data))
            return;

        adsBoxDataOnScene.Dictionary[Id] = new AdsBoxOnSceneData(
            transform.position.AsVectorData(),
            Type,
            SceneBuiltInItem,

            _resourceConfig.Type,
            _dropResourceCount
            );
    }

    public override void ReadProgress(GameProgress progress)
    {
        var adsBoxDataOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].AdsBoxesDatas.AdsBoxesOnScene;

        // we are build-in-scene object and it is first start of level
        if (!adsBoxDataOnScene.Dictionary.TryGetValue(Id, out var myState))
            return;

        // restore state
        transform.position = myState.Position.AsUnityVector();
    }

    protected override void OnRewardedVideoEndSuccessfully()
    {
        base.OnRewardedVideoEndSuccessfully();

        //Logger.Log($"[AdsBox] {gameObject.name} - drop resources");
        DropResource();
    }

    private void DropResource()
    {
        View.PlayDropResourceSound();
        var dropData = DropData.Get(transform.position, DropSettings, _dropResourceCount, out int notFittedInPacksCount);

        for (int i = 0; i < dropData.Count; i++)
        {
            Resource resource = _resourceFactory.Get(transform.position, Quaternion.identity);
            resource.Init(_resourceConfig, dropData[i].ResourceInPackCount);

            resource.MoveAfterDrop(dropData[i]);
        }
    }

    protected override void Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}