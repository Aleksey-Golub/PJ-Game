using Code.Services;
using UnityEngine;

public class Converter : MonoBehaviour, IResourceConsumer
{
    [SerializeField] private ConverterView _view;

    [SerializeField] private ConverterConfig _config;
    [SerializeField] private ResourceConfig _needResourceConfig;
    [SerializeField] private int _singleUpload = 5;
    [SerializeField] private int _maxUpload = 25;
    [SerializeField] private float _converTime = 10f;
    [SerializeField] private int _preferedConsumedValue = -1;
    [SerializeField] private Transform _transitionalResourceFinal;

    [SerializeField] private ResourceConfig _dropResourceConfig;
    [SerializeField] private DropSettings _dropSettings = DropSettings.Default;
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

    private void Start()
    {
        var resourceFactory = AllServices.Container.Single<IResourceFactory>();
        var progressService = AllServices.Container.Single<IPersistentProgressService>();
        var audio = AllServices.Container.Single<IAudioService>();
        var effectFactory = AllServices.Container.Single<IEffectFactory>();

        Construct(resourceFactory, progressService, audio, effectFactory);
        Init();
    }

    private void Update()
    {
        OnUpdate(Time.deltaTime);
    }
    private void OnDestroy()
    {
        if (_config && _progressService != null)
        {
            _progressService.Progress.PlayerProgress.UpgradeItemsProgress.Changed -= OnUpgradeItemsProgressChanged;
        }
    }

    private void Construct(IResourceFactory resourceFactory, IPersistentProgressService progressService, IAudioService audio, IEffectFactory effectFactory)
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

    internal void Init()
    {
        _currentUpload = 0;
        _currentPreUpload = 0;
        _timer = 0;

        _view.Init(_needResourceConfig.Sprite, _currentUpload, _dropResourceConfig.Sprite);
        _view.ShowNeeds(_singleUpload);
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

    private void DropResource()
    {
        _view.PlayDropResourceSound();

        var dropData = DropData.Get(transform.position, _dropSettings, _dropCount, out int notFittedInPacksCount);

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
}
