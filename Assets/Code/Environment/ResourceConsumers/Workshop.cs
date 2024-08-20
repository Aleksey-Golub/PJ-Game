using UnityEngine;

public class Workshop : MonoBehaviour, IResourceConsumer
{
    [SerializeField] private ResourceConsumerView _view;
    [SerializeField] private Collider2D _collider;

    [SerializeField] private ResourceConfig _needResourceConfig;
    [SerializeField] private int _needResourceCount = 1;

    [SerializeField] private GameObject _spawnObject;

    private int _currentNeedResourceCount;

    public bool CanInteract => _currentNeedResourceCount != 0;

    private void Start()
    {
        Construct();
        Init();
    }

    private void Construct()
    {
    }

    internal void Init()
    {
        _currentNeedResourceCount = _needResourceCount;

        _view.Init(_needResourceConfig.Sprite, _currentNeedResourceCount, null);
    }

    public ResourceConsumerNeeds GetNeeds()
    {
        return new ResourceConsumerNeeds()
        {
            ResourceType = _needResourceConfig.Type,
            CurrentNeedResourceCount = _currentNeedResourceCount
        };
    }

    public void Consume(int value)
    {
        _currentNeedResourceCount -= value;
        _view.ShowNeeds(_currentNeedResourceCount);

        if (_currentNeedResourceCount == 0)
        {
            _view.ShowHitAnimation();
            DropObject();
            Exhaust();
        }
    }
    private void Exhaust()
    {
        _view.ShowExhaust();

        Invoke(nameof(DisableCollider), 1f);
    }

    private void DisableCollider()
    {
        _collider.enabled = false;
    }

    private void DropObject()
    {
        _view.PlayDropResourceSound();
        _view.ShowHitEffect();

        Instantiate(_spawnObject, transform.position, Quaternion.identity);
    }
}
