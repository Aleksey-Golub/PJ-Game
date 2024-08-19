using UnityEngine;

internal class Workbench : MonoBehaviour, IResourceConsumer
{
    [SerializeField] private WorkbenchView _view;
    [SerializeField] private Collider2D _collider;

    [SerializeField] private ResourceConfig _needResourceConfig;
    [SerializeField] private int _needResourceCount = 1;

    [SerializeField] private ResourceConfig _dropConfig;
    [SerializeField] private DropSettings _dropSettings = DropSettings.Default;
    [SerializeField] private int _dropCount = 1;

    private int _currentNeedResourceCount;
    private ResourceFactory _factory;

    public bool CanInteract => _currentNeedResourceCount != 0;

    private void Start()
    {
        var factory = ResourceFactory.Instance;
        Construct(factory);
        Init();
    }

    private void Construct(ResourceFactory factory)
    {
        _factory = factory;
    }

    internal void Init()
    {
        _currentNeedResourceCount = _needResourceCount;

        _view.Init(_needResourceConfig.Sprite, _currentNeedResourceCount, _dropConfig.Sprite);
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

        var dropData = DropData.Get(transform.position, _dropSettings, _dropCount, out int notFittedInPacksCount);

        for (int i = 0; i < dropData.Count; i++)
        {
            Resource dropObject = _factory.Get(transform.position, Quaternion.identity);
            dropObject.Init(_dropConfig, dropData[i].ResourceInPackCount);

            dropObject.MoveAfterDrop(dropData[i]);
        }
    }
}

