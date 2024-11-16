using Code.Data;
using Code.Infrastructure;
using Code.Services;
using UnityEngine;

[SelectionBase]
public class Converter : MonoBehaviour, IResourceConsumer, ISavedProgressReader, ISavedProgressWriter, IUniqueIdHolder, IPossibleSceneBuiltInItem, ICreatedByIdGameObject
{
    [field: SerializeField] public bool SceneBuiltInItem { get; private set; }
    [field: SerializeField] public UniqueId UniqueId { get; private set; }
    [SerializeField] private ConverterView _view;

    [SerializeField] private ConverterConfig _config;
    [SerializeField] private ResourceConfig _needResourceConfig;
    [SerializeField] private int _singleUpload = 5;
    [Tooltip("Used when config is null")]
    [SerializeField] private int _maxUpload = 25;
    [SerializeField] private float _converTime = 10f;
    [SerializeField] private int _preferedConsumedValue = -1;
    [SerializeField] private Transform _transitionalResourceFinal;

    [SerializeField] private ResourceConfig _dropResourceConfig;
    [field: SerializeField] public DropSettings DropSettings { get; private set; } = DropSettings.Default;
    [SerializeField] private int _dropCount = 1;

    private float _timer;
    private int _currentUpload;
    private int _currentPreUpload;
    private IResourceFactory _resourceFactory;
    private IPersistentProgressService _progressService;

    public bool CanInteract => _currentUpload < GetMaxUpload() && _currentPreUpload < GetMaxUpload();
    public int PreferedConsumedValue => _preferedConsumedValue;
    public int FreeSpace => GetMaxUpload() - _currentPreUpload;
    public Vector3 TransitionalResourceFinalPosition => _transitionalResourceFinal.position;

    private string Id => UniqueId.Id;

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
            Init();

            gameFactory.RegisterProgressWatchersExternal(gameObject);
        }
    }

    private void Update() => OnUpdate(Time.deltaTime);

    private void OnDestroy()
    {
        if (_config && _progressService != null)
        {
            _progressService.Progress.PlayerProgress.UpgradeItemsProgress.Changed -= OnUpgradeItemsProgressChanged;
        }
    }

    public void Construct(IResourceFactory resourceFactory, IPersistentProgressService progressService, IAudioService audio, IEffectFactory effectFactory)
    {
        _resourceFactory = resourceFactory;
        _progressService = progressService;

        _view.Construct(audio, effectFactory);

        if (_config)
        {
            UnlockUpgrade(_progressService);
            _progressService.Progress.PlayerProgress.UpgradeItemsProgress.Changed += OnUpgradeItemsProgressChanged;
        }

        void UnlockUpgrade(IPersistentProgressService progressService)
        {
            Code.Data.UpgradeItemsProgress upgradeItemsProgress = progressService.Progress.PlayerProgress.UpgradeItemsProgress;
            string id = _config.ID;
            upgradeItemsProgress.TryGet(id, out int value);
            if (value == 0)
                upgradeItemsProgress.Set(id, 1);
        }
    }

    public void Init()
    {
        _currentUpload = 0;
        _currentPreUpload = 0;
        _timer = 0;

        _view.Init(_needResourceConfig.Sprite, _currentUpload, _dropResourceConfig.Sprite);
        _view.ShowNeeds(_singleUpload, 0);
        _view.ShowUpload(_currentUpload, GetMaxUpload());
        _view.ShowProgress(_timer, _converTime);
    }

    public void WriteToProgress(GameProgress progress)
    {
        var convertersOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].ConvertersDatas.ConvertersOnScene;

        // just to optimize
        convertersOnScene.Dictionary.TryGetValue(Id, out var data);
        if (data != null && !HasChangesBetweenSavedStateAndCurrentState(data))
            return;

        convertersOnScene.Dictionary[Id] = new ConverterOnSceneData(
            transform.position.AsVectorData(),
            SceneBuiltInItem,
            _config.Type,
            _currentUpload,
            _timer
            );
    }

    public void ReadProgress(GameProgress progress)
    {
        var convertersOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].ConvertersDatas.ConvertersOnScene;

        // we are in scene object and it is first start of level
        if (!convertersOnScene.Dictionary.TryGetValue(Id, out var myState))
            return;

        // restore state
        transform.position = myState.Position.AsUnityVector();
        _currentUpload = myState.CurrentUpload;
        _timer = myState.Timer;
        _currentPreUpload = _currentUpload;

        _view.ShowUpload(_currentUpload, GetMaxUpload());
        _view.ShowProgress(_timer, _converTime);
    }

    private void OnUpdate(float deltaTime)
    {
        if (_currentUpload < _singleUpload)
            return;

        _timer += deltaTime;

        if (_timer >= _converTime)
        {
            _timer = 0;

            _currentUpload -= _singleUpload;
            _currentPreUpload -= _singleUpload;
            _view.ShowUpload(_currentUpload, GetMaxUpload());
            DropResource();
        }

        _view.ShowProgress(_timer, _converTime);
    }

    public ResourceConsumerNeeds GetNeeds()
    {
        return new ResourceConsumerNeeds()
        {
            ResourceType = _needResourceConfig.Type,
            CurrentNeedResourceCount = GetMaxUpload() - _currentUpload
        };
    }

    public void Consume(int value)
    {
        _currentUpload += value;
        _view.ShowUpload(_currentUpload, GetMaxUpload());
    }

    public void ApplyPreUpload(int consumedValue)
    {
        _currentPreUpload += consumedValue;
    }

    private bool HasChangesBetweenSavedStateAndCurrentState(ConverterOnSceneData data)
    {
        return
            data.Timer != _timer ||
            data.CurrentUpload != _currentUpload ||
            data.Position.AsUnityVector() != transform.position
            ;
    }

    private void DropResource()
    {
        _view.PlayDropResourceSound();

        var dropData = DropData.Get(transform.position, DropSettings, _dropCount, out int notFittedInPacksCount);

        for (int i = 0; i < dropData.Count; i++)
        {
            Resource dropObject = _resourceFactory.Get(transform.position, Quaternion.identity);
            dropObject.Init(_dropResourceConfig, dropData[i].ResourceInPackCount);

            dropObject.MoveAfterDrop(dropData[i]);
        }
    }

    private int GetMaxUpload()
    {
        if (!_config)
            return _maxUpload;
        else
        {
            _progressService.Progress.PlayerProgress.UpgradeItemsProgress.TryGet(_config.ID, out int level);
            return (int)_config.GetUpgradeData(level).Value;
        }
    }

    private void OnUpgradeItemsProgressChanged(string itemId, int newValue)
    {
        if (itemId == _config.ID)
            _view.ShowUpload(_currentUpload, GetMaxUpload());
    }

    void ICreatedByIdGameObject.Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}
