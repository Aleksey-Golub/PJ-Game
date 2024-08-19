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

    private ResourceFactory _resourceFactory;

    protected float _restorationTimer = 0;
    protected int _currentHitPoints = 0;

    internal bool IsDied => _currentHitPoints <= 0;

    internal ToolType NeedToolType => _needToolType;

    private void Start()
    {
        var resourceFactory = ResourceFactory.Instance;
        Construct(resourceFactory);
    }

    private void Construct(ResourceFactory resourceFactory)
    {
        _resourceFactory = resourceFactory;

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
            gameObject.SetActive(false);

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

        if (_currentHitPoints <= 0)
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

        var dropData = DropData.Get(transform.position, _dropSettings, _dropResourceCount, out int notFittedInPacksCount);

        for (int i = 0; i < dropData.Count; i++)
        {
            Resource resource = _resourceFactory.Get(transform.position, Quaternion.identity);
            resource.Init(_resourceConfig, dropData[i].ResourceInPackCount);

            resource.MoveAfterDrop(dropData[i]);
        }
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
