using Code.Data;
using Code.Infrastructure;
using Code.Services;
using System;
using UnityEngine;

[SelectionBase]
public class ResourceSource : MonoBehaviour, ISavedProgressReader, ISavedProgressWriter, IUniqueIdHolder, IPossibleSceneBuiltInItem, ICreatedByIdGameObject
{
    private const int PLAYER_DAMAGE = 1;

    [field: SerializeField] public bool SceneBuiltInItem { get; private set; }
    [SerializeField] private ResourceSourceType _type;
    [field: SerializeField] public UniqueId UniqueId { get; private set; }
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private ResourceSourceViewBase _view;

    [Header("Settings")]
    [SerializeField] private ToolType _needToolType;
    [SerializeField] private ResourceConfig _resourceConfig;
    [SerializeField, Min(1)] private int _dropResourceCount = 1;
    [SerializeField] protected int _hitPoints = 1;
    [SerializeField] protected float _restoreTime = 10;
    [field: SerializeField] public DropSettings DropSettings { get; private set; } = DropSettings.Default;

    private IResourceFactory _resourceFactory;
    private IDropCountCalculatorService _dropCalculator;
    protected float _restorationTimer = 0;
    protected int _currentHitPoints = 0;

    private string Id => UniqueId.Id;
    protected bool IsSingleUse => _restoreTime < 0;
    internal bool IsDied => _currentHitPoints <= 0;
    internal ToolType NeedToolType => _needToolType;

    internal event Action<ResourceSource> Dropped;

    private void Start()
    {
        if (SceneBuiltInItem)
        {
            var resourceFactory = AllServices.Container.Single<IResourceFactory>();
            var dropCountCalculatorService = AllServices.Container.Single<IDropCountCalculatorService>();
            var audio = AllServices.Container.Single<IAudioService>();
            var effectFactory = AllServices.Container.Single<IEffectFactory>();
            var gameFactory = AllServices.Container.Single<IGameFactory>();

            Construct(resourceFactory, dropCountCalculatorService, audio, effectFactory);
            gameFactory.RegisterProgressWatchersExternal(gameObject);
        }
    }

    public void Construct(
        IResourceFactory resourceFactory,
        IDropCountCalculatorService dropCountCalculatorService,
        IAudioService audio,
        IEffectFactory effectFactory
        )
    {
        _resourceFactory = resourceFactory;
        _dropCalculator = dropCountCalculatorService;

        _view.Construct(audio, effectFactory);

        RestoreHP(_hitPoints);

        if (IsSingleUse)
            enabled = false;
    }

    public void Init(ResourceConfig resourceConfig, int dropResourceCount)
    {
        _resourceConfig = resourceConfig;
        _dropResourceCount = dropResourceCount;
    }

    public void WriteToProgress(GameProgress progress)
    {
        var resourceSourceOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].ResourceSourcesDatas.ResourceSourcesOnScene;

        // just to optimize
        resourceSourceOnScene.Dictionary.TryGetValue(Id, out var data);
        if (data != null && !HasChangesBetweenSavedStateAndCurrentState(data))
            return;

        resourceSourceOnScene.Dictionary[Id] = new ResourceSourceOnSceneData(
            transform.position.AsVectorData(),
            _type,
            _resourceConfig.Type,
            _dropResourceCount,
            _restorationTimer,
            _currentHitPoints,
            SceneBuiltInItem
            );
    }

    public void ReadProgress(GameProgress progress)
    {
        var rSourceOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].ResourceSourcesDatas.ResourceSourcesOnScene;

        // we are in scene object and it is first start of level
        if (!rSourceOnScene.Dictionary.TryGetValue(Id, out var myState))
            return;

        // restore state
        transform.position = myState.Position.AsUnityVector();
        _restorationTimer = myState.RestorationTimer;
        _currentHitPoints = myState.CurrentHitPoints;

        _view.ShowHP(_currentHitPoints, _hitPoints);
        if (IsDied)
        {
            Exhaust();
            if (IsSingleUse)
                InactivateSelf();
        }
    }

    private void Update()
    {
        OnUpdate(Time.deltaTime);
    }

    protected virtual void OnUpdate(float deltaTime)
    {
        if (!IsDied)
            return;

        if (IsSingleUse)
            return;

        _restorationTimer += deltaTime;

        if (_restorationTimer >= _restoreTime)
        {
            _restorationTimer = 0;
            Restore();
        }
    }

    internal void Interact()
    {
        //Logger.Log($"Interact with {gameObject.name} {Time.frameCount}");

        _currentHitPoints -= PLAYER_DAMAGE;
        _view.ShowHP(_currentHitPoints, _hitPoints);
        _view.ShowHitEffect();
        _view.PlayHitSound();

        if (DropConditionIsTrue())
            DropResource();

        if (IsDied)
        {
            Exhaust();

            if (IsSingleUse)
                InactivateSelf();

            return;
        }

        _view.ShowHitAnimation();
    }

    protected virtual bool DropConditionIsTrue() => IsDied;

    private void DropResource()
    {
        _view.PlayDropResourceSound();
        int count = _needToolType == ToolType.None ? _dropResourceCount : _dropCalculator.Calculate(_dropResourceCount, _resourceConfig.Type, NeedToolType);
        var dropData = DropData.Get(transform.position, DropSettings, count, out int notFittedInPacksCount);

        for (int i = 0; i < dropData.Count; i++)
        {
            Resource resource = _resourceFactory.Get(transform.position, Quaternion.identity);
            resource.Init(_resourceConfig, dropData[i].ResourceInPackCount);

            resource.MoveAfterDrop(dropData[i]);
        }

        Dropped?.Invoke(this);
    }

    protected virtual void Exhaust()
    {
        _collider2D.enabled = false;
        _view.ShowExhaust();
    }

    private void Restore()
    {
        RestoreHP(_hitPoints);
        _collider2D.enabled = true;
        _view.ShowWhole();
    }

    protected void RestoreHP(int value)
    {
        _currentHitPoints += value;
        _view.ShowHP(_currentHitPoints, _hitPoints);
    }

    private void InactivateSelf()
    {
        gameObject.SetActive(false);
    }

    private bool HasChangesBetweenSavedStateAndCurrentState(ResourceSourceOnSceneData data)
    {
        return
            data.CurrentHitPoints != _currentHitPoints ||
            data.RestorationTimer != _restorationTimer ||
            data.Position.AsUnityVector() != transform.position
            ;
    }

    void ICreatedByIdGameObject.Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}

public enum ResourceSourceType
{
    None = 0,
    GrassBush = 1,
    Pine = 2,
    Rock = 3,
    FruitBush = 4,
    Slime = 5,
    Pot = 6,
}
