using Code.Data;
using Code.Infrastructure;
using Code.Services;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class Converter : MonoBehaviour, IResourceConsumer, ISavedProgressReader, ISavedProgressWriter, IUniqueIdHolder, IPossibleSceneBuiltInItem, ICreatedByIdGameObject
{
    [field: SerializeField] public bool Available { get; private set; } = true;
    [field: SerializeField] public bool SceneBuiltInItem { get; private set; }
    [field: SerializeField] public UniqueId UniqueId { get; private set; }
    [SerializeField] private ConverterView _view;

    [SerializeField] private ConverterConfig _config;
    [SerializeField] private float _converTime = 10f;

    [Header("Needs")]
    [SerializeField] private ResourceConsumerNeedSettings[] _needSettings;

    [Header("Drop")]
    [SerializeField] private DropResourceSettings[] _dropResourceSettings;
    [field: SerializeField] public DropSettings DropSettings { get; private set; } = DropSettings.Default;

    private float _timer;
    private Dictionary <ResourceType, ResourceConsumerNeedData> _needData;
    private IResourceFactory _resourceFactory;
    private IPersistentProgressService _progressService;

    public bool CanInteract => 
        Available 
        && _needData.Any(kvp => kvp.Value.CurrentUpload < GetMaxUpload(kvp.Key)) 
        && _needData.Any(kvp => kvp.Value.CurrentPreUpload < GetMaxUpload(kvp.Key));
    public ConverterType Type => _config.Type;

    private string Id => UniqueId.Id;
    private List<ResourceConsumerNeeds> _needs;

#if DEBUG && FAST_DEBUG
    private void Awake()
    {
        _converTime = 1f;
    }
#endif

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
        _needs = GetInitialNeeds();
        _needData = _needSettings.ToDictionary(s => s.NeedResourceConfig.Type, s => new ResourceConsumerNeedData(s));

        _resourceFactory = resourceFactory;
        _progressService = progressService;

        _view.Construct(audio, effectFactory);

        if (_config)
        {
            UnlockUpgrade(_progressService);
            _progressService.Progress.PlayerProgress.UpgradeItemsProgress.Changed += OnUpgradeItemsProgressChanged;
        }

        // locals
        void UnlockUpgrade(IPersistentProgressService progressService)
        {
            Code.Data.UpgradeItemsProgress upgradeItemsProgress = progressService.Progress.PlayerProgress.UpgradeItemsProgress;
            string id = _config.ID;
            upgradeItemsProgress.TryGet(id, out int value);
            if (value == 0)
                upgradeItemsProgress.Set(id, 1);
        }

        List<ResourceConsumerNeeds> GetInitialNeeds()
        {
            List<ResourceConsumerNeeds> result = new(_needSettings.Length);

            foreach (var needSetting in _needSettings)
                result.Add(new ResourceConsumerNeeds(needSetting.NeedResourceConfig.Type));

            return result;
        }
    }

    public void Init()
    {
        foreach (var needData in _needData)
        {
            needData.Value.CurrentUpload = 0;
            needData.Value.CurrentPreUpload = 0;
        }

        _timer = 0;

        _view.Init(_needSettings, _needData, _dropResourceSettings);
        ShowAllNeeds();
        ShowAllUploads();
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
            Available,
            _config.Type,
            GetCurrentUploads(),
            _timer
            );

        // locals
        List<UploadData> GetCurrentUploads()
        {
            List<UploadData> result = new();

            foreach (var setting in _needSettings)
            {
                var resourceType = setting.NeedResourceConfig.Type;
                result.Add(new UploadData(resourceType, _needData[resourceType].CurrentUpload));
            }

            return result;
        }
    }

    public void ReadProgress(GameProgress progress)
    {
        var convertersOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].ConvertersDatas.ConvertersOnScene;

        // we are build-in-scene object and it is first start of level
        if (!convertersOnScene.Dictionary.TryGetValue(Id, out var myState))
            return;

        // restore state
        transform.position = myState.Position.AsUnityVector();
        SetCurrentUpload(myState);
        _timer = myState.Timer;
        SetCurrentPreupload(myState);

        ShowAllUploads();
        _view.ShowProgress(_timer, _converTime);

        // locals
        void SetCurrentUpload(ConverterOnSceneData myState)
        {
            if (myState.CurrentUploads.Count == 0)
            {
                // we use old save version
                foreach (var needData in _needData.Values)
                    needData.CurrentUpload = myState.CurrentUpload;

                return;
            }

            foreach (var uploadData in myState.CurrentUploads)
                _needData[uploadData.ResourceType].CurrentUpload = uploadData.CurrentUpload;
        }

        void SetCurrentPreupload(ConverterOnSceneData myState)
        {
            if (myState.CurrentUploads.Count == 0)
            {
                // we use old save version
                foreach (var needData in _needData.Values)
                    needData.CurrentPreUpload = myState.CurrentUpload;

                return;
            }

            foreach (var uploadData in myState.CurrentUploads)
                _needData[uploadData.ResourceType].CurrentPreUpload = uploadData.CurrentUpload;
        }
    }

    private void OnUpdate(float deltaTime)
    {
        // check if we can produce resource
        if (_needData.Any(kvp => kvp.Value.CurrentUpload < kvp.Value.Settings.SingleUpload))
            return;

        _timer += deltaTime;

        if (_timer >= _converTime)
        {
            _timer = 0;

            foreach (var needData in _needData.Values)
            {
                needData.CurrentUpload -= needData.Settings.SingleUpload;
                needData.CurrentPreUpload -= needData.Settings.SingleUpload;
            }

            ShowAllUploads();
            DropResource();
        }

        _view.ShowProgress(_timer, _converTime);
    }

    public int GetPreferedConsumedValue(ResourceType resourceType) => _needData[resourceType].Settings.PreferedConsumedValue;
    public int GetFreeSpace(ResourceType resourceType) => GetMaxUpload(resourceType) - _needData[resourceType].CurrentPreUpload;
    public Vector3 GetTransitionalResourceFinalPosition(ResourceType resourceType) => _needData[resourceType].Settings.TransitionalResourceFinal.position;

    public List<ResourceConsumerNeeds> GetNeeds()
    {
        foreach (var need in _needs)
        {
            ResourceType resourceType = need.ResourceType;
            need.CurrentNeedResourceCount = GetMaxUpload(resourceType) - _needData[resourceType].CurrentUpload;
        }

        return _needs;
    }

    public void Consume(ResourceType resourceType, int value)
    {
        _needData[resourceType].CurrentUpload += value;
        ShowAllUploads();
    }

    public void ApplyPreUpload(ResourceType resourceType, int consumedValue)
    {
        _needData[resourceType].CurrentPreUpload += consumedValue;
    }

    public void SetAvailable()
    {
        Available = true;
        ShowAllNeeds();
    }

    private bool HasChangesBetweenSavedStateAndCurrentState(ConverterOnSceneData data)
    {
        return
            data.Timer != _timer ||
            HasUploadChanges(data) ||
            data.Position.AsUnityVector() != transform.position
            ;

        // locals
        bool HasUploadChanges(ConverterOnSceneData data)
        {
            if (data.CurrentUploads.Count == 0)
            {
                // first new save after old save version
                return true;
            }

            foreach (var uploadData in data.CurrentUploads)
            {
                if (uploadData.CurrentUpload != _needData[uploadData.ResourceType].CurrentUpload)
                    return true;
            }

            return false;
        }
    }

    private void DropResource()
    {
        _view.PlayDropResourceSound();

        foreach (var dropResourceSetting in _dropResourceSettings)
        {
            var dropData = DropData.Get(transform.position, DropSettings, dropResourceSetting.DropCount, out int notFittedInPacksCount);

            for (int i = 0; i < dropData.Count; i++)
            {
                Resource dropObject = _resourceFactory.Get(transform.position, Quaternion.identity);
                dropObject.Init(dropResourceSetting.DropResourceConfig, dropData[i].ResourceInPackCount);

                dropObject.MoveAfterDrop(dropData[i]);
            }
        }
    }
    
    private int GetMaxUpload(ResourceType resourceType)
    {
        if (!_config)
            return _needData[resourceType].Settings.MaxUpload;
        else
        {
            string converterId = _config.ID;
            _progressService.Progress.PlayerProgress.UpgradeItemsProgress.TryGet(converterId, out int level);

            UpgradeStaticData upgradeData = _config.GetUpgradeData(level);
            int resourceIndex = _needData[resourceType].Settings.NeedIndex;

            switch (resourceIndex)
            {
                case 0:
                    return (int)upgradeData.Value;
                case 1:
                    return (int)upgradeData.Value1;
                default:
                    Logger.LogError($"[Converter] upgradeData not found for resource='{resourceType}' in '{converterId}'");
                    return _needData[resourceType].Settings.MaxUpload;
            }
        }
    }

    private void OnUpgradeItemsProgressChanged(string itemId, int newValue)
    {
        if (itemId == _config.ID)
            ShowAllUploads();
    }

    private void ShowAllNeeds()
    {
        int totalNeedResourcesCount = _needSettings.Sum(s => s.SingleUpload);
        foreach (var needSetting in _needSettings)
            _view.ShowNeeds(needSetting.NeedResourceConfig.Type, needSetting.SingleUpload, 0, Available, totalNeedResourcesCount);
    }

    private void ShowAllUploads()
    {
        foreach (var needData in _needData)
            _view.ShowUpload(needData.Key, needData.Value.CurrentUpload, GetMaxUpload);
    }

    void ICreatedByIdGameObject.Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}
