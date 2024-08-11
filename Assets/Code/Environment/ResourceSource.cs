using UnityEngine;

internal class ResourceSource : MonoBehaviour, IInteractable
{
    private const int PLAYER_DAMAGE = 1;

    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private ResourceSourceView _view;

    [Header("Settings")]
    [SerializeField] private ToolType _needToolType;
    [SerializeField] private ResourceConfig _resourceConfig;
    [SerializeField, Min(1)] private int _dropResourceCount = 1;
    [SerializeField] private int _hitPoints = 1;
    [SerializeField] private float _restoreTime = 10;
    [SerializeField] private DropSettings _dropSettings = new() { DropRadius = 1.3f, MoveAfterDropTime = 0.6f, DropStrategy = DropStrategy.RandomInsideCircle};

    private ResourceFactory _resourceFactory;

    private float _restorationTimer = 0;
    private int _currentHitPoints = 0;

    private bool IsDied => _currentHitPoints <= 0;

    internal ToolType NeedToolType => _needToolType;

    private void Construct(ResourceFactory resourceFactory)
    {
        _resourceFactory = resourceFactory;

        RestoreHP();
    }

    private void Start()
    {
        var resourceFactory = ResourceFactory.Instance;
        Construct(resourceFactory);
    }

    private void Update()
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

    public void Interact()
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

    private void DropResource()
    {
        _view.PlayDropResourceSound();

        var dropData = DropData.Get(transform.position, _dropSettings, _dropResourceCount);

        for (int i = 0; i < _dropResourceCount; i++)
        {
            Resource resource = _resourceFactory.Get(transform.position, Quaternion.identity);
            resource.Init(_resourceConfig);

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
        RestoreHP();
        _collider2D.enabled = true;
        _view.ShowWhole();
    }

    private void RestoreHP()
    {
        _currentHitPoints = _hitPoints;
        _view.ShowHP(_currentHitPoints, _hitPoints);
    }
}
