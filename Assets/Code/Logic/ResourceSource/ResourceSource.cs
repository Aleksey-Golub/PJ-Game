using Code.Services;
using System;
using UnityEngine;

internal class ResourceSource : MonoBehaviour
{
    protected const int PLAYER_DAMAGE = 1;

    [SerializeField] private Collider2D _collider2D;
    [SerializeField] protected ResourceSourceViewBase _view;

    [Header("Settings")]
    [SerializeField] private ToolType _needToolType;
    [SerializeField] private ResourceConfig _resourceConfig;
    [SerializeField, Min(1)] private int _dropResourceCount = 1;
    [SerializeField] protected int _hitPoints = 1;
    [SerializeField] protected float _restoreTime = 10;
    [SerializeField] private DropSettings _dropSettings = DropSettings.Default;

    private IResourceFactory _resourceFactory;
    private IDropCountCalculatorService _dropCalculator;
    protected float _restorationTimer = 0;
    protected int _currentHitPoints = 0;

    internal bool IsDied => _currentHitPoints <= 0;
    internal ToolType NeedToolType => _needToolType;

    internal event Action<ResourceSource> Dropped;

    private void Start()
    {
        var resourceFactory = AllServices.Container.Single<IResourceFactory>();
        var dropCountCalculatorService = AllServices.Container.Single<IDropCountCalculatorService>();
        var audio = AllServices.Container.Single<IAudioService>();
        var effectFactory = AllServices.Container.Single<IEffectFactory>();

        Construct(resourceFactory, dropCountCalculatorService, audio, effectFactory);
    }

    private void Construct(
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
    }

    private void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        if (!IsDied)
            return;

        if (_restoreTime < 0)
        {
            gameObject.SetActive(false);
        }

        _restorationTimer += Time.deltaTime;

        if (_restorationTimer >= _restoreTime)
        {
            _restorationTimer = 0;
            Restore();
        }
    }

    internal virtual void Interact()
    {
        //Logger.Log($"Interact with {gameObject.name} {Time.frameCount}");

        _currentHitPoints -= PLAYER_DAMAGE;
        _view.ShowHP(_currentHitPoints, _hitPoints);
        _view.ShowHitEffect();
        _view.PlayHitSound();

        if (IsDied)
        {
            Exhaust();
            DropResource();
            return;
        }

        _view.ShowHitAnimation();
    }

    protected void DropResource()
    {
        _view.PlayDropResourceSound();
        int count = _needToolType == ToolType.None ? _dropResourceCount : _dropCalculator.Calculate(_dropResourceCount, _resourceConfig.Type, NeedToolType);
        var dropData = DropData.Get(transform.position, _dropSettings, count, out int notFittedInPacksCount);

        for (int i = 0; i < dropData.Count; i++)
        {
            Resource resource = _resourceFactory.Get(transform.position, Quaternion.identity);
            resource.Init(_resourceConfig, dropData[i].ResourceInPackCount);

            resource.MoveAfterDrop(dropData[i]);
        }

        Dropped?.Invoke(this);
    }

    private void Exhaust()
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
}
