using Code.Data;
using Code.Infrastructure;
using Code.Services;
using UnityEngine;

[SelectionBase]
public class ResourceStorage : MonoBehaviour, ISavedProgressReader, ISavedProgressWriter, IUniqueIdHolder, IPossibleSceneBuiltInItem
{
    [field: SerializeField] public bool SceneBuiltInItem { get; private set; }
    [field: SerializeField] public UniqueId UniqueId { get; private set; }
    [SerializeField] private ResourceStorageView _view;

    [Header("Settings")]
    [SerializeField] private ResourceStorageConfig _config;
    [SerializeField] private ToolType _needToolType = ToolType.None;
    [SerializeField] private ResourceConfig _resourceConfig;
    [Tooltip("Used when item is unUpgradable")]
    [SerializeField, Min(1)] private int _maxResourceCount = 1;
    [SerializeField] private int _startResourceCount = 1;
    [SerializeField] private float _restoreTime = 10;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private DropSettings _dropSettings = DropSettings.Default;

    private IResourceFactory _resourceFactory;
    private IPersistentProgressService _progressService;
    private IExhaustStrategy _exhaust;
    private float _restorationTimer = 0;
    private int _currentResourceCount;

    private string Id => UniqueId.Id;
    private bool IsFull => _currentResourceCount >= GetMaxResourceCount();
    private bool IsSingleUse => _restoreTime < 0;
    internal bool CanInteract => _currentResourceCount > 0;
    internal ToolType NeedToolType => _needToolType;

    private void Start()
    {
        if (SceneBuiltInItem)
        {
            var resourceFactory = AllServices.Container.Single<IResourceFactory>();
            var progressService = AllServices.Container.Single<IPersistentProgressService>();
            var audio = AllServices.Container.Single<IAudioService>();
            var effectFactory = AllServices.Container.Single<IEffectFactory>();
            var gameFactory = AllServices.Container.Single<IGameFactory>();

            Construct(resourceFactory, progressService, audio, effectFactory);
            gameFactory.RegisterProgressWatchers(gameObject);
        }
    }

    public void Construct(IResourceFactory resourceFactory, IPersistentProgressService progressService, IAudioService audio, IEffectFactory effectFactory)
    {
        _resourceFactory = resourceFactory;
        _progressService = progressService;
        _exhaust = new ExhaustStrategy(this, _collider);

        _view.Construct(audio, effectFactory);

        if (_config.IsUpgradable)
        {
            UnlockUpgrade(_progressService);
            _progressService.Progress.PlayerProgress.UpgradeItemsProgress.Changed += OnUpgradeItemsProgressChanged;
        }

        _currentResourceCount = _startResourceCount;
        _view.ShowResourceCount(_currentResourceCount, GetMaxResourceCount());
        _view.ShowWhole();

        if (IsSingleUse)
            enabled = false;

        void UnlockUpgrade(IPersistentProgressService progressService)
        {
            Code.Data.UpgradeItemsProgress upgradeItemsProgress = progressService.Progress.PlayerProgress.UpgradeItemsProgress;
            string id = _config.ID;
            upgradeItemsProgress.TryGet(id, out int value);
            if (value == 0)
                upgradeItemsProgress.Set(id, 1);
        }
    }

    public void Init(ResourceConfig resourceConfig)
    {
        _resourceConfig = resourceConfig;
    }

    public void WriteToProgress(GameProgress progress)
    {
        var resourceStorageOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].ResourceStoragesDatas.ResourceStoragesOnScene;

        // just to optimize
        resourceStorageOnScene.Dictionary.TryGetValue(Id, out var data);
        if (data != null && !HasChangesBetweenSavedStateAndCurrentState(data))
            return;

        resourceStorageOnScene.Dictionary[Id] = new ResourceStorageOnSceneData(
            transform.position.AsVectorData(),
            _config.Type,
            _resourceConfig.Type,
            _currentResourceCount,
            _restorationTimer,
            SceneBuiltInItem
            );
    }

    public void ReadProgress(GameProgress progress)
    {
        var rStoragesOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].ResourceStoragesDatas.ResourceStoragesOnScene;

        // we are in scene object and it is first start of level
        if (!rStoragesOnScene.Dictionary.TryGetValue(Id, out var myState))
            return;

        // restore state
        transform.position = myState.Position.AsUnityVector();
        _currentResourceCount = myState.CurrentResourceCount;
        _restorationTimer = myState.RestorationTimer;

        _view.ShowResourceCount(_currentResourceCount, GetMaxResourceCount());
        if (!CanInteract)
        {
            _view.ShowExhaust();

            if (IsSingleUse)
                _exhaust.ExhaustImmediately();
        }
    }

    private void Update()
    {
        OnUpdate(Time.deltaTime);
    }

    private void OnUpdate(float deltaTime)
    {
        if (IsFull)
            return;

        if (IsSingleUse)
            return;

        _restorationTimer += deltaTime;

        if (_restorationTimer >= _restoreTime && _currentResourceCount < GetMaxResourceCount())
        {
            _restorationTimer = 0;
            Restore(1);
        }
    }

    private void OnDestroy()
    {
        if (_config.IsUpgradable && _progressService != null)
        {
            _progressService.Progress.PlayerProgress.UpgradeItemsProgress.Changed -= OnUpgradeItemsProgressChanged;
        }
    }

    internal virtual void Interact()
    {
        //Logger.Log($"Interact with {gameObject.name} {Time.frameCount}");

        _view.ShowHitEffect();
        _view.PlayHitSound();
        _view.ShowHitAnimation();

        DropResource();
        Exhaust();
    }

    private void DropResource()
    {
        _view.PlayDropResourceSound();

        var dropData = DropData.Get(transform.position, _dropSettings, _currentResourceCount, out int notFittedInPacksCount);
        Restore(notFittedInPacksCount);

        for (int i = 0; i < dropData.Count; i++)
        {
            Resource resource = _resourceFactory.Get(transform.position, Quaternion.identity);
            resource.Init(_resourceConfig, dropData[i].ResourceInPackCount);

            resource.MoveAfterDrop(dropData[i]);
        }
    }

    private void Exhaust()
    {
        _currentResourceCount = 0;
        _view.ShowResourceCount(_currentResourceCount, GetMaxResourceCount());
        _view.ShowExhaust();

        if (IsSingleUse)
            _exhaust.ExhaustDelayed(1f);
    }

    private void Restore(int value)
    {
        if (value == 0)
            return;

        _currentResourceCount += value;
        _view.ShowResourceCount(_currentResourceCount, GetMaxResourceCount());
        _view.ShowWhole();
    }

    private int GetMaxResourceCount()
    {
        if (!_config.IsUpgradable)
            return _maxResourceCount;
        else
        {
            _progressService.Progress.PlayerProgress.UpgradeItemsProgress.TryGet(_config.ID, out int level);
            return (int)_config.GetUpgradeData(level).Value;
        }
    }

    private void OnUpgradeItemsProgressChanged(string itemId, int newValue)
    {
        if (itemId == _config.ID)
            _view.ShowResourceCount(_currentResourceCount, GetMaxResourceCount());
    }

    private bool HasChangesBetweenSavedStateAndCurrentState(ResourceStorageOnSceneData data)
    {
        return
            data.RestorationTimer != _restorationTimer ||
            data.CurrentResourceCount != _currentResourceCount ||
            data.Position.AsUnityVector() != transform.position
            ;
    }
}
