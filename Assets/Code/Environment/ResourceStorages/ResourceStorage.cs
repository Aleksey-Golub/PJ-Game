using System;
using UnityEngine;

internal class ResourceStorage : MonoBehaviour
{
    [SerializeField] private ResourceStorageView _view;

    [Header("Settings")]
    [SerializeField] private ResourceStorageConfig _config;
    [SerializeField] private ToolType _needToolType = ToolType.None;
    [SerializeField] private ResourceConfig _resourceConfig;
    [SerializeField, Min(1)] private int _dropResourceCount = 1;
    [SerializeField] private int _startResourceCount = 1;
    [SerializeField] private float _restoreTime = 10;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private DropSettings _dropSettings = DropSettings.Default;

    private ResourceFactory _resourceFactory;
    private PersistentProgressService _progressService;
    private float _restorationTimer = 0;
    private int _currentResourceCount;

    private bool IsFull => _currentResourceCount >= GetDropResourceCount();
    private bool IsSingleUse => _restoreTime < 0;
    internal bool CanInteract => _currentResourceCount > 0;
    internal ToolType NeedToolType => _needToolType;
    private void Start()
    {
        var resourceFactory = ResourceFactory.Instance;
        var progressService = PersistentProgressService.Instance;
        Construct(resourceFactory, progressService);
    }

    private void Construct(ResourceFactory resourceFactory, PersistentProgressService progressService)
    {
        _resourceFactory = resourceFactory;
        _progressService = progressService;

        if (_config)
        {
            UnlockUpgrade(_progressService);
            _progressService.Progress.PlayerProgress.UpgradeItemsProgress.Changed += OnUpgradeItemsProgressChanged;
        }

        _currentResourceCount = _startResourceCount;
        _view.ShowResourceCount(_currentResourceCount, GetDropResourceCount());
        _view.ShowWhole();

        void UnlockUpgrade(PersistentProgressService progressService)
        {
            Assets.Code.Data.UpgradeItemsProgress upgradeItemsProgress = progressService.Progress.PlayerProgress.UpgradeItemsProgress;
            string id = _config.ID;
            upgradeItemsProgress.TryGet(id, out int value);
            if (value == 0)
                upgradeItemsProgress.Set(id, 1);
        }
    }

    private void Update()
    {
        OnUpdate();
    }

    private void OnUpdate()
    {
        if (IsSingleUse)
        {
            enabled = (false);
            return;
        }

        if (IsFull)
            return;

        _restorationTimer += Time.deltaTime;

        if (_restorationTimer >= _restoreTime && _currentResourceCount < GetDropResourceCount())
        {
            _restorationTimer = 0;
            Restore(1);
        }
    }

    private void OnDestroy()
    {
        if (_config)
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
        _view.ShowResourceCount(_currentResourceCount, GetDropResourceCount());
        _view.ShowExhaust();

        if (IsSingleUse)
            Invoke(nameof(DisableCollider), 1f);
    }

    private void DisableCollider()
    {
        _collider.enabled = false;
    }

    private void Restore(int value)
    {
        if (value == 0)
            return;

        _currentResourceCount += value;
        _view.ShowResourceCount(_currentResourceCount, GetDropResourceCount());
        _view.ShowWhole();
    }

    private int GetDropResourceCount()
    {
        if (!_config)
            return _dropResourceCount;
        else
        {
            _progressService.Progress.PlayerProgress.UpgradeItemsProgress.TryGet(_config.ID, out int level);
            return (int)_config.GetUpgradeData(level).Value;
        }
    }

    private void OnUpgradeItemsProgressChanged(string itemId, int newValue)
    {
        if (itemId == _config.ID)
            _view.ShowResourceCount(_currentResourceCount, GetDropResourceCount());
    }
}
